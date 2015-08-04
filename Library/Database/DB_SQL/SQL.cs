// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SQL.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using MySql.Data.MySqlClient;

#endregion

namespace Regulus.Database.SQL
{
	public class Database : IDisposable
	{
		private MySqlConnection _Connect;

		private string _ConnectParams;

		void IDisposable.Dispose()
		{
			this.Disconnect();
		}

		public void Connect(string host, string user, string password, string database_name)
		{
			var connect = "Server = " + host + "; UserId = " + user + "; Password = " + password + "; Database = "
			              + database_name + string.Empty;
			this._ConnectParams = connect;
			this._Connect = this._CreateConnector(connect);
		}

		private MySqlConnection _CreateConnector(string connect)
		{
			var c = new MySqlConnection(connect);

			try
			{
				c.Open();
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
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
			if (this._Connect != null)
			{
				this._Connect.Close();
			}

			this._Connect = null;
		}

		public void ExecuteNonQuery(string command_str)
		{
			try
			{
				var cmd = new MySqlCommand(command_str, this._Connect);
				cmd.ExecuteNonQuery();
			}
			catch
			{
				this._Connect = this._CreateConnector(this._ConnectParams);
				this.ExecuteNonQuery(command_str);
			}
		}

		public MySqlDataReader Execute(string command_str)
		{
			try
			{
				var cmd = new MySqlCommand(command_str, this._Connect);
				return cmd.ExecuteReader();
			}
			catch
			{
				this._Connect = this._CreateConnector(this._ConnectParams);
				return this.Execute(command_str);
			}
		}
	}
}