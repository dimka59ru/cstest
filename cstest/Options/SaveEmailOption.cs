using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest.Options
{
    public class SaveEmailOption : IOption
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public void Action()
        {
            SaveEmailToSettings();
        }

        private void SaveEmailToSettings()
        {
            if (IsEmailValid(Value))
            {
                Console.WriteLine("Сохраняем email в настройки");
                Properties.Settings.Default.Email = Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine("Email невалидный");
            }
        }

        private static bool IsEmailValid(string email)
        {
            // TODO добавит проверку email на валидность

            return !string.IsNullOrEmpty(email);
        }
    }
}
