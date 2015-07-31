// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageWriter.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the PackageWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public class PackageWriter
	{
		public event CheckSourceCallback CheckSourceEvent
		{
			add { this._CheckSourceEvent += value; }

			remove { this._CheckSourceEvent -= value; }
		}

		public event OnErrorCallback ErrorEvent;

		private const int _HeadSize = 4;

		private readonly PowerRegulator _PowerRegulator;

		private IAsyncResult _AsyncResult;

		private byte[] _Buffer;

		private CheckSourceCallback _CheckSourceEvent;

		private Socket _Socket;

		private volatile bool _Stop;

		/// <summary>
		/// Initializes a new instance of the <see cref="PackageWriter"/> class. 
		/// </summary>
		/// <param name="low_fps">
		/// 保證最低fps
		/// </param>
		public PackageWriter(int low_fps)
		{
			this._PowerRegulator = new PowerRegulator(low_fps);
		}

		public PackageWriter()
		{
			this._PowerRegulator = new PowerRegulator();
		}

		public delegate Package[] CheckSourceCallback();

		public void Start(Socket socket)
		{
			this._Stop = false;
			this._Socket = socket;

			this._Write();
		}

		private void _Write()
		{
			try
			{
				var pkgs = this._CheckSourceEvent();

				this._Buffer = this._CreateBuffer(pkgs);
				this._PowerRegulator.Operate(this._Buffer.Length);

				this._AsyncResult = this._Socket.BeginSendTo(this._Buffer, 0, this._Buffer.Length, 0, this._Socket.RemoteEndPoint, 
					this._WriteCompletion, null);
			}
			catch (SystemException e)
			{
				Singleton<Log>.Instance.WriteInfo(string.Format("PackageWriter Error Write {0}.", e));
				if (this.ErrorEvent != null)
				{
					this.ErrorEvent();
				}
			}
		}

		private void _WriteCompletion(IAsyncResult ar)
		{
			try
			{
				if (this._Stop == false)
				{
					var sendSize = this._Socket.EndSendTo(ar);

					this._Write();
				}
			}
			catch (SystemException e)
			{
				Singleton<Log>.Instance.WriteInfo(string.Format("PackageWriter Error WriteCompletion {0}.", e));
				if (this.ErrorEvent != null)
				{
					this.ErrorEvent();
				}
			}
		}

		private byte[] _CreateBuffer(Package[] packages)
		{
			var buffers = from p in packages select TypeHelper.Serializer(p);

			// Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serializer to Buffer size {0}", buffers.Sum( b => b.Length )));
			using (var stream = new MemoryStream())
			{
				foreach (var buffer in buffers)
				{
					stream.Write(BitConverter.GetBytes(buffer.Length), 0, PackageWriter._HeadSize);
					stream.Write(buffer, 0, buffer.Length);
				}

				return stream.ToArray();
			}
		}

		public void Stop()
		{
			this._Stop = true;

			// _Socket = null;
			this._CheckSourceEvent = this._Empty;
		}

		private Package[] _Empty()
		{
			return new Package[0];
		}
	}
}