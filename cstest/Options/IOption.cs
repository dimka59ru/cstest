using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest.Options
{
    public interface IOption
    {
        string Key { get; set; }
        string Value { get; set; }

        void Action();
    }
}
