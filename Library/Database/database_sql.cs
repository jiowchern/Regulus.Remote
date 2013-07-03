using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus
{
    namespace SQL
    {
        using MySql.Data;
        using MySql.Data.MySqlClient;
        public class Database : IDisposable
        {
            MySqlConnection _Connect;

            public void Connect(string host, string user, string password, string database_name)
            {
                _Connect = new MySqlConnection("server=" + host + ";uid=" + user + ";pwd=" + password + ";database=" + database_name);

                try
                {
                    _Connect.Open();
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

            }

            public void Disconnect()
            {
                if (_Connect != null)
                    _Connect.Close();
                _Connect = null;
            }

			public void ExecuteSQL(string command_str)
			{
				var cmd = _Connect.CreateCommand();
				using (var reader = cmd.ExecuteReader())
				{
					
				}
			}

            void IDisposable.Dispose()
            {
                Disconnect();
            }
        }
    }
}
