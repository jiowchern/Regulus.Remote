namespace Regulus.Utility
{
	public class LogWritter
	{
		private readonly Log.RecordCallback AsyncRecord;

		private readonly string message;

		public LogWritter(string message, Log.RecordCallback AsyncRecord)
		{
			// TODO: Complete member initialization
			this.message = message;
			this.AsyncRecord = AsyncRecord;
		}

		internal void Write()
		{
			AsyncRecord(message);
		}
	}
}
