using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Native
{
    public class PackageWriter
    {
        public delegate Package[] CheckSourceCallback();
        CheckSourceCallback _CheckSourceEvent;
        public event CheckSourceCallback CheckSourceEvent
        {
            add 
            {
                
                _CheckSourceEvent += value;
            }

            remove
            {
                
                _CheckSourceEvent -= value;
            }
        }
        const int _HeadSize = 4;
        System.Net.Sockets.Socket _Socket;
        
        private byte[] _Buffer;
        private IAsyncResult _AsyncResult;

        public event OnErrorCallback ErrorEvent;
        
        volatile bool _Stop;
        Regulus.Utility.PowerRegulator _PowerRegulator;
        
        public PackageWriter()
        {
            
            _PowerRegulator = new Utility.PowerRegulator();
        }
        public void Start(System.Net.Sockets.Socket socket)
        {
            _Stop = false;
            _Socket = socket;

            
            _Write();

        }

        private void _Write()
        {
            try
            {
                
                Package[] pkgs = _CheckSourceEvent();

                _Buffer = _CreateBuffer(pkgs);
                _PowerRegulator.Operate(_Buffer.Length);                
                    
                _AsyncResult = _Socket.BeginSendTo(_Buffer, 0, _Buffer.Length, 0, _Socket.RemoteEndPoint, _WriteCompletion, null);
                
            }
            catch (SystemException e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("PackageWriter Error Write {0}.", e.ToString()));
                if (ErrorEvent != null)
                    ErrorEvent();
            }
        }

        private void _WriteCompletion(IAsyncResult ar)
        {
            try
            {
                
                if (_Stop == false)
                {
                    
                    var sendSize = _Socket.EndSendTo(ar);
                    
                                        
                    _Write();
                }
                
            }
            catch (SystemException e)
            {
                Regulus.Utility.Log.Instance.WriteInfo(string.Format("PackageWriter Error WriteCompletion {0}.", e.ToString()));
                if (ErrorEvent != null)
                    ErrorEvent();
            }
            
        }

        byte[] _CreateBuffer(Package[] packages)
        {            
            var buffers = from p in packages select Regulus.Serializer.TypeHelper.Serializer<Package>(p);
            //Regulus.Utility.Log.Instance.WriteDebug(string.Format("Serializer to Buffer size {0}", buffers.Sum( b => b.Length )));
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {

                foreach (var buffer in buffers)
                {
                    stream.Write(System.BitConverter.GetBytes((int)buffer.Length), 0, _HeadSize);
                    stream.Write(buffer, 0, buffer.Length);
                }
                
                return stream.ToArray();
            }
        }

        


        public void Stop()
        {
            _Stop = true;
            
            //_Socket = null;
            _CheckSourceEvent = _Empty;
        }

        private Package[] _Empty()
        {
            return new Package[0];
        }
    }
}
