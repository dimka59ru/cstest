using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest
{
    interface IService
    {
        string Name { get; set; }
        string ConnectionString { get; set; }

        bool Connection();
    }
}
