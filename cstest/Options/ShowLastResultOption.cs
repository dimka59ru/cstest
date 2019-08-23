using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cstest.Options
{
    public class ShowLastResultOption : IOption
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public void Action()
        {
            var messages = ReadMessagesFromXmlFile(Program.xmlFilePath); // Можно путь к файлу задать в настройках
            ShowMessagesOnScreen(messages);
        }

        private static void ShowMessagesOnScreen(List<string> messages)
        {
            foreach (var message in messages)
                Console.WriteLine(message);
        }

        private static List<string> ReadMessagesFromXmlFile(string path)
        {
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

            using (StreamReader file = new StreamReader(path))
            {
                List<string> messages = (List<string>)reader.Deserialize(file);
                return messages;
            }
        }
    }
}
