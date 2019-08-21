using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cstest
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Запущено без параметров");
                string uri = Properties.Settings.Default.Uri;
                

                string checkUriMessage = "";
                try
                {
                    bool checkUriResult = UriAvailabilityCheck(uri);
                    if (checkUriResult)
                        checkUriMessage = $"{DateTime.Now}: Сайт {uri} доступен";
                    else
                        checkUriMessage = $"{DateTime.Now}: Сайт {uri} недоступен";
                }
                catch (UriFormatException ex)
                {
                    checkUriMessage = $"{DateTime.Now}: Проверка сайта {uri} неудалась. Ошибка {ex.Message}";
                }

                SendEmail(checkUriMessage);
                SaveResultUriAvailabilityCheckToFile(checkUriMessage);

                Console.ReadKey();
                return 1;
            }

            string[] args2 = { "--uri", "ya.ru"};


            // Парсим командную строку
            List<Option> options = ParseArguments(args);             

            if (options.Any())
            {
                ProcessOptions(options);
            }
            else
            {
                Console.WriteLine("Переданы неизвестные параметры");
            }            

            Console.ReadKey();
            return 0;
        }

        private static void SaveResultUriAvailabilityCheckToFile(string message)
        {            
            using (StreamWriter file = File.CreateText("result.json"))
            {                
                var contentsToWriteToFile = JsonConvert.SerializeObject(message, Formatting.Indented);
                file.WriteLine(contentsToWriteToFile);                
            }            
        }

        private static void SendEmail(string message)
        {
            string email = Properties.Settings.Default.Email;
            Console.WriteLine($"Email будет отправлен на {email}");
            Console.WriteLine(message);
        }
        private static bool UriAvailabilityCheck(string uri)
        {
            bool result = false;
            
            WebRequest request = WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }
            
            return result;
        }

        private static void ProcessOptions(List<Option> options)
        {
            foreach (var option in options)
            {
                if (option.Name == "--print")
                {
                    Console.WriteLine("Печатаем результаты на экран");

                    using (StreamReader sr = new StreamReader("result.json"))
                    {
                        string line = sr.ReadToEnd();
                        var res = JsonConvert.DeserializeObject<string>(line);
                        Console.WriteLine(res);
                    }                    
                }

                if (option.Name == "--uri")
                {
                    string uri = option.Value;
                    SaveUrlToSettings(uri);
                }

                if (option.Name == "--email")
                {
                    string email = option.Value;
                    SaveEmailToSettings(email);
                }

                Properties.Settings.Default.Save();
            }            

        }

        private static void SaveEmailToSettings(string email)
        {
            if (IsEmailValid(email))
            {
                Console.WriteLine("Сохрняем email в настройки");
                Properties.Settings.Default.Email = email;
            }
            else
            {
                Console.WriteLine("Email невалидный");
            }
        }

        private static void SaveUrlToSettings(string uri)
        {
            if (IsUrlValid(uri))
            {
                Console.WriteLine("Сохрняем uri в настройки");
                Properties.Settings.Default.Uri = uri;
            }
            else
            {
                Console.WriteLine("Uri невалидный. Пример: http://ya.ru");
            }
        }

        private static bool IsUrlValid(string uri)
        {
            if (!string.IsNullOrEmpty(uri) && Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                return true;
            }
            return false;
        }

        private static bool IsEmailValid(string email)
        {
            // TODO добавит проверку email на валидность

            return !string.IsNullOrEmpty(email);
        }

        private static List<Option> ParseArguments(string[] args)
        {
            bool argWasRead = true;
            
            List<Option> options = new List<Option>();
            Option option = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--")) // Если аргумент начинается с --
                {
                    option = new Option();
                    option.Name = arg;
                    options.Add(option);

                    argWasRead = false;                    
                    continue;             // Переходим к следующему аргументу
                }

                if (!argWasRead)
                {
                    option.Value = arg;                    
                    argWasRead = true;                   
                }
                
            }
            return options;
        }
    }
}
