using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;


using Regulus.Utility;

namespace Regulus.Remoting
{
	public class PackageWriter
	{
		public delegate Package[] CheckSourceCallback();

		public event CheckSourceCallback CheckSourceEvent
		{
			add { _CheckSourceEvent += value; }
			remove { _CheckSourceEvent -= value; }
		}

		public event OnErrorCallback ErrorEvent;

		private const int _HeadSize = 4;

		private readonly PowerRegulator _PowerRegulator;

	    private AutoPowerRegulator _AutoPowerRegulator;

        private IAsyncResult _AsyncResult;

		private byte[] _Buffer;

		private CheckSourceCallback _CheckSourceEvent;

		private Socket _Socket;

		private volatile bool _Stop;

		/// <summary>
		///     Initializes a new instance of the <see cref="PackageWriter" /> class.
		/// </summary>
		/// <param name="low_fps">
		///     保證最低fps
		/// </param>
		public PackageWriter(int low_fps)
		{
            _PowerRegulator = new PowerRegulator(low_fps);
            _AutoPowerRegulator = new AutoPowerRegulator(_PowerRegulator);
        }

		public PackageWriter()
		{
			_PowerRegulator = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_PowerRegulator);
        }

		public void Start(Socket socket)
		{
			_Stop = false;
			_Socket = socket;

			_Write();
		}

		private void _Write()
		{
			try
			{
				var pkgs = _CheckSourceEvent();

				_Buffer = _CreateBuffer(pkgs);
                _AutoPowerRegulator.Operate();

                _AsyncResult = _Socket.BeginSendTo(
					_Buffer, 
					0, 
					_Buffer.Length, 
					0, 
					_Socket.RemoteEndPoint, 
					_WriteCompletion, 
					null);
			}
			catch(SystemException e)
			{
				Singleton<Log>.Instance.WriteInfo(string.Format("PackageWriter Error Write {0}.", e));
				if(ErrorEvent != null)
				{
					ErrorEvent();
				}
			}
		}

		private void _WriteCompletion(IAsyncResult ar)
		{
			try
			{
				if(_Stop == false)
				{
					var sendSize = _Socket.EndSendTo(ar);

					_Write();
				}
			}
			catch(SystemException e)
			{
				Singleton<Log>.Instance.WriteInfo(string.Format("PackageWriter Error WriteCompletion {0}.", e));
				if(ErrorEvent != null)
				{
					ErrorEvent();
				}
			}
		}

		private byte[] _CreateBuffer(Package[] packages)
		{
			var buffers = from p in packages select TypeHelper.Serializer(p);

			// Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serializer to Buffer size {0}", buffers.Sum( b => b.Length )));
			using(var stream = new MemoryStream())
			{
				foreach(var buffer in buffers)
				{
					stream.Write(BitConverter.GetBytes(buffer.Length), 0, PackageWriter._HeadSize);
					stream.Write(buffer, 0, buffer.Length);
				}

				return stream.ToArray();
			}
		}

		public void Stop()
		{
			_Stop = true;

			// _Socket = null;
			_CheckSourceEvent = _Empty;
		}

		private Package[] _Empty()
		{
			return new Package[0];
		}
	}
}
