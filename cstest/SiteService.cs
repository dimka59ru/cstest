using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cstest
{
    public class SiteService : IService
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public bool Connection()
        {
            Console.WriteLine($"Подключаемся к '{Name}'");

            WebRequest request = WebRequest.Create(ConnectionString);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
