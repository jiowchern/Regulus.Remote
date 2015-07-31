// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageReader.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the OnErrorCallback type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Net.Sockets;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	public delegate void OnErrorCallback();

	public delegate void OnByteDataCallback(byte[] bytes);

	public delegate void OnPackageCallback(Package package);

	public class PackageReader
	{
		public event OnPackageCallback DoneEvent
		{
			add { this._DoneEvent += value; }
			remove { this._DoneEvent -= value; }
		}

		public event OnErrorCallback ErrorEvent;

		private enum Status
		{
			Begin, 

			End
		}

		private const int _HeadSize = 4;

		private OnPackageCallback _DoneEvent;

		private SocketReader _Reader;

		private Socket _Socket;

		private volatile bool _Stop;

		public void Start(Socket socket)
		{
			Singleton<Log>.Instance.WriteInfo("pakcage read start.");
			this._Stop = false;
			this._Socket = socket;
			this._ReadHead();
		}

		private void _ReadHead()
		{
			this._Reader = new SocketReader(this._Socket);
			this._Reader.DoneEvent += this._ReadBody;
			this._Reader.ErrorEvent += this.ErrorEvent;

			this._Reader.Read(PackageReader._HeadSize);
		}

		private void _ReadBody(byte[] bytes)
		{
			var bodySize = BitConverter.ToInt32(bytes, 0);
			this._Reader = new SocketReader(this._Socket);
			this._Reader.DoneEvent += this._Package;
			this._Reader.ErrorEvent += this.ErrorEvent;

			this._Reader.Read(bodySize);
		}

		private void _Package(byte[] bytes)
		{
			var pkg = TypeHelper.Deserialize<Package>(bytes);

			this._DoneEvent.Invoke(pkg);

			if (this._Stop == false)
			{
				this._ReadHead();
			}
		}

		public void Stop()
		{
			this._Stop = true;
			this._Socket = null;

			Singleton<Log>.Instance.WriteInfo("pakcage read stop.");
		}
	}

	internal class SocketReader
	{
		public event OnByteDataCallback DoneEvent;

		public event OnErrorCallback ErrorEvent;

		private readonly Socket _Socket;

		private byte[] _Buffer;

		private int _Offset;

		public SocketReader(Socket _Socket)
		{
			this._Socket = _Socket;
		}

		internal void Read(int size)
		{
			this._Offset = 0;
			this._Buffer = new byte[size];
			try
			{
				this._Socket.BeginReceive(this._Buffer, this._Offset, this._Buffer.Length - this._Offset, 0, this._Readed, null);
			}
			catch (SystemException e)
			{
				if (this.ErrorEvent != null)
				{
					this.ErrorEvent();
				}
			}
		}

		private void _Readed(IAsyncResult ar)
		{
			try
			{
				var readSize = this._Socket.EndReceive(ar);
				this._Offset += readSize;

				if (readSize == 0)
				{
					this.ErrorEvent();
				}
				else if (this._Offset == this._Buffer.Length)
				{
					this.DoneEvent(this._Buffer);
				}
				else
				{
					this._Socket.BeginReceive(this._Buffer, this._Offset, this._Buffer.Length - this._Offset, SocketFlags.None, 
						this._Readed, null);
				}
			}
			catch (SystemException e)
			{
				if (this.ErrorEvent != null)
				{
					this.ErrorEvent();
				}
			}
			finally
			{
			}
		}
	}
}