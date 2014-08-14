using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus
{
    namespace SQL
    {
        using MySql.Data.MySqlClient;
        
        public class Database : IDisposable
        {
            MySqlConnection _Connect;

            public void Connect(string host, string user, string password, string database_name)
            {                
                
                var connect = "Server = " + host + "; UserId = " + user + "; Password = " + password + "; Database = " + database_name + "";
                _Connect = _CreateConnector(connect);

            }

            private MySqlConnection _CreateConnector(string connect)
            {
                var c = new MySqlConnection(connect);

                try
                {
                    c.Open();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            throw new SystemException("MySQL:無法連線到資料庫.");
                        case 1045:
                            throw new SystemException("MySQL:使用者帳號或密碼錯誤.");

                    }
                    throw new SystemException("MySQL:連線失敗.代號:" + ex.Number.ToString());
                }
                return c;
            }

            public void Disconnect()
            {
                if (_Connect != null)
                    _Connect.Close();
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
                    _Connect = _CreateConnector(_Connect.ConnectionString);
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
                    _Connect = _CreateConnector(_Connect.ConnectionString);
                    return Execute(command_str);
                }
                
			}

            void IDisposable.Dispose()
            {
                Disconnect();
            }
        }
    }
}
