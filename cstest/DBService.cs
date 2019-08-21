using System;
using System.Collections.Generic;
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
            return false;
        }
    }
}
