using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest
{
    class DBService : IService
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }        

        public bool Connection()
        {
            Console.WriteLine($"Подключаемся к '{Name}'");
            SqlConnection cnn = new SqlConnection(ConnectionString);
            try
            {
                cnn.Open();
            }
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                cnn.Close();
            }

            return true;            
        }
    }
}
