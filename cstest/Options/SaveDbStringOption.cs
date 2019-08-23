using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest.Options
{
    public class SaveDbStringOption : IOption
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public void Action()
        {
            SaveDbStringToSettings();
        }

        private void SaveDbStringToSettings()
        {
            if (IsdBStringValid(Value))
            {
                Console.WriteLine("Сохарняем dBString в настройки");
                Properties.Settings.Default.DbConnectionString = Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine("dBString невалидный");
            }
        }

        private static bool IsdBStringValid(string dBString)
        {
            return !string.IsNullOrEmpty(dBString);
        }
    }
}
