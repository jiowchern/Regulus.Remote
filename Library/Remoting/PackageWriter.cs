using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

using Regulus.Serialization;
using Regulus.Utility;

namespace Regulus.Remoting
{
	public class PackageWriter<TPackage>
	{
	    private readonly ISerializer _Serializer;
		

		public event OnErrorCallback ErrorEvent;

		

		private readonly PowerRegulator _PowerRegulator;

	    private readonly AutoPowerRegulator _AutoPowerRegulator;

		private byte[] _Buffer;



	    private readonly System.Collections.Generic.List<TPackage> _Packages;

		private Socket _Socket;

		private volatile bool _Stop;
	    private bool _Idle;

	    /// <summary>
	    ///     Initializes a new instance of the <see cref="PackageWriter" /> class.
	    /// </summary>
	    /// <param name="low_fps">
	    ///     保證最低fps
	    /// </param>
	    /// <param name="serializer">序列化物件</param>
	    

		public PackageWriter( ISerializer serializer)
		{
            _Packages = new List<TPackage>();
            _Serializer = serializer;
            _PowerRegulator = new PowerRegulator();
            _AutoPowerRegulator = new AutoPowerRegulator(_PowerRegulator);
	        _Idle = true;
		}

		public void Start(Socket socket)
		{
			_Stop = false;
			_Socket = socket;
		
		}

	    public void Push(TPackage[] packages)
	    {
	        lock (_Packages)
	        {
                _Packages.AddRange(packages);                
	        }
	        _Write();


	    }
		private void _Write()
		{
			try
			{

			    lock (_Packages)
			    {
                    _Buffer = _CreateBuffer(_Packages.ToArray());
                    _Packages.Clear();
                }
			    if (_Buffer.Length <= 0)
			    {
			        return;
			    }
                _AutoPowerRegulator.Operate();
                NetworkMonitor.Instance.Write.Set(_Buffer.Length);
                _Socket.BeginSend(_Buffer, 0, _Buffer.Length, SocketFlags.None, _WriteCompletion, null);
                
			}
			catch(SystemException e)
			{
			    var info = string.Format("PackageWriter Error Write {0}.", _Socket.Connected);
                Singleton<Log>.Instance.WriteInfo(info);
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

		private byte[] _CreateBuffer(TPackage[] packages)
		{
			var buffers = from p in packages select _Serializer.Serialize(p);

			// Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serialize to Buffer size {0}", buffers.Sum( b => b.Length )));
			using(var stream = new MemoryStream())
			{
				foreach(var buffer in buffers)
				{
				    var len = buffer.Length;
				    var lenCount = Regulus.Serialization.Varint.GetByteCount(len);
				    var lenBuffer = new byte[lenCount];
				    Regulus.Serialization.Varint.NumberToBuffer(lenBuffer, 0, len);
					stream.Write(lenBuffer, 0, lenBuffer.Length);
					stream.Write(buffer, 0, buffer.Length);
				}

				return stream.ToArray();
			}
		}

		public void Stop()
		{
			_Stop = true;

			
			
		}
		
	}
}
