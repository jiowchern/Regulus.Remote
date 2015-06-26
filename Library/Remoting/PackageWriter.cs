using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Native
{
    public class PackageWriter
    {
        public delegate Package[] CheckSourceCallback();
        public event CheckSourceCallback CheckSourceEvent;
        const int _HeadSize = 4;
        System.Net.Sockets.Socket _Socket;
        
        private byte[] _Buffer;
        private IAsyncResult _AsyncResult;

        public event OnErrorCallback ErrorEvent;
        
        volatile bool _Stop;
        Regulus.Utility.SpinWait _Wait;

        
        public PackageWriter()
        {
            _Wait = new Utility.SpinWait();
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
                Package[] pkgs ;
                pkgs = CheckSourceEvent();

                if (pkgs.Length == 0)
                {
                    _Wait.SpinOnce();
                    if (_Wait.Count > 1000)
                        System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    _Wait.Reset();                
                }                        

                _Buffer = _CreateBuffer(pkgs);

                //Regulus.Utility.Log.Instance.WriteDebug(string.Format("0.Write {0}", _Buffer.Length));
                _AsyncResult = _Socket.BeginSend(_Buffer, 0, _Buffer.Length, 0, _WriteCompletion, null);
                //Regulus.Utility.Log.Instance.WriteDebug(string.Format("1.Write {0}", _Buffer.Length));
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
                    //Regulus.Utility.Log.Instance.WriteDebug(string.Format("1.Write Completion {0}", _Buffer.Length ));
                    _Socket.EndSend(ar);
                    
                    //Regulus.Utility.Log.Instance.WriteDebug(string.Format("2.Write Completion {0}", _Buffer.Length));
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
            //_DebugLoadSoulLog(packages);
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

        private void _DebugLoadSoulLog(Package[] packages)
        {
            foreach(var p in  packages)
            {
                if(p.Code == (byte)ServerToClientOpCode.LoadSoul)
                {
                    Regulus.Utility.Log.Instance.WriteDebug(string.Format("DebugLoadSoulLog "));
                }
            }
        }


        public void Stop()
        {
            _Stop = true;
            
            _Socket = null;
            CheckSourceEvent = _Empty;
        }

        private Package[] _Empty()
        {
            return new Package[0];
        }
    }
}
