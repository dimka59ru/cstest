using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest.Options
{
    public class SaveUriOption : IOption
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public void Action()
        {
            SaveUriToSettings();
        }

        private void SaveUriToSettings()
        {
            if (IsUriValid(Value))
            {
                Console.WriteLine("Сохраняем uri в настройки");
                Properties.Settings.Default.Uri = Value;
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine("Uri невалидный. Пример: http://ya.ru");
            }
        }

        private static bool IsUriValid(string uri)
        {
            if (!string.IsNullOrEmpty(uri) && Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                return true;
            }
            return false;
        }
    }
}
