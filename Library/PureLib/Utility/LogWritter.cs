using System;
using System.Linq.Expressions;

namespace Regulus.Utility
{
	public class LogWritter
	{
	    public enum TYPE
	    {
	        INFO,DEBUG
	    }
		private readonly Log.RecordCallback _AsyncRecord;

		private readonly Expression<Func<string>> _Message;
	    private readonly TYPE _Type;
        public LogWritter(TYPE type,Expression<Func<string>> message, Log.RecordCallback AsyncRecord)
        {
            _Type = type;

            this._Message = message;
			this._AsyncRecord = AsyncRecord;
		}

		internal void Write()
		{

		    var message = _GetMessage();
			_AsyncRecord(message);
		}

	    private string _GetMessage()
	    {
	        var message = _Message.Compile()();

            if (_Type == TYPE.INFO)
	            return string.Format("[{1}][Info]{0}", message, System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
	        else if (_Type == TYPE.DEBUG)
	        {
	            return string.Format("[{2}][Debug]{0}\r\n{1}", message, System.Environment.StackTrace,
	                System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"));
	        }
            throw new NotImplementedException("log type " + _Type);
	    }
	}
}
