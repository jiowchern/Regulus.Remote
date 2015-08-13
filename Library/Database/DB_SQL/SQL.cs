using System;


using MySql.Data.MySqlClient;

namespace Regulus.Database.DB_SQL
{
	public class Database : IDisposable
	{
		private MySqlConnection _Connect;

		private string _ConnectParams;

		void IDisposable.Dispose()
		{
			Disconnect();
		}

		public void Connect(string host, string user, string password, string database_name)
		{
			var connect = "Server = " + host + "; UserId = " + user + "; Password = " + password + "; Database = "
			              + database_name + string.Empty;
			_ConnectParams = connect;
			_Connect = _CreateConnector(connect);
		}

		private MySqlConnection _CreateConnector(string connect)
		{
			var c = new MySqlConnection(connect);

			try
			{
				c.Open();
			}
			catch(MySqlException ex)
			{
				switch(ex.Number)
				{
					case 0:
						throw new SystemException("MySQL:無法連線到資料庫.");
					case 1045:
						throw new SystemException("MySQL:使用者帳號或密碼錯誤.");
				}

				throw new SystemException("MySQL:連線失敗.代號:" + ex.Number);
			}

			return c;
		}

		public void Disconnect()
		{
			if(_Connect != null)
			{
				_Connect.Close();
			}

			_Connect = null;
		}

		public void ExecuteNonQuery(string command_str)
		{
			try
			{
				var cmd = new MySqlCommand(command_str, _Connect);
				cmd.ExecuteNonQuery();
			}
			catch
			{
				_Connect = _CreateConnector(_ConnectParams);
				ExecuteNonQuery(command_str);
			}
		}

		public MySqlDataReader Execute(string command_str)
		{
			try
			{
				var cmd = new MySqlCommand(command_str, _Connect);
				return cmd.ExecuteReader();
			}
			catch
			{
				_Connect = _CreateConnector(_ConnectParams);
				return Execute(command_str);
			}
		}
	}
}
