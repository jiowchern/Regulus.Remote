using System;
using System.Linq.Expressions;

namespace Regulus.Utility
{
    public class LogWritter
    {
        public enum TYPE
        {
            INFO, DEBUG
        }
        private readonly Log.RecordCallback _AsyncRecord;

        private readonly Expression<Func<string>> _Message;
        private readonly TYPE _Type;
        private readonly DateTime _Time;

        public LogWritter(TYPE type, Expression<Func<string>> message, Log.RecordCallback AsyncRecord)
        {
            _Time = System.DateTime.Now;
            _Type = type;

            this._Message = message;
            this._AsyncRecord = AsyncRecord;
        }

        internal void Write()
        {

            string message = _GetMessage();
            _AsyncRecord(message);
        }

        private string _GetMessage()
        {
            string message = _Message.Compile()();

            if (_Type == TYPE.INFO)
                return string.Format("[{1}][Info]{0}", message, _Time.ToString(@"yyyy/MM/dd_hh:mm:ss"));
            else if (_Type == TYPE.DEBUG)
            {
                return string.Format("[{2}][Debug]{0}\r\n{1}", message, System.Environment.StackTrace,
                    _Time.ToString("yyyy/MM/dd_hh:mm:ss"));
            }
            throw new NotImplementedException("log type " + _Type);
        }
    }
}
