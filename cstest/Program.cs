using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace cstest
{
    class Program
    {
        static readonly string xmlFilePath = "result.xml";
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Запущено без параметров");

                List<IService> services = new List<IService>();

                SiteService site = new SiteService();
                site.Name = "Сайт ya.ru";
                site.ConnectionString = Properties.Settings.Default.Uri;                
                services.Add(site);

                DBService dB = new DBService();
                dB.Name = "БД MSSQL";
                dB.ConnectionString = Properties.Settings.Default.DbConnectionString;
                services.Add(dB);

                List<string> messages = new List<string>();

                foreach(var service in services)
                {
                    string result = CheckConnection(service);
                    messages.Add(result);
                }

                SaveMessagesToXmlFile(messages, xmlFilePath);
                SendEmail(messages);

                //Console.WriteLine("Нажмите любую клавишу для завершения");
                //Console.ReadKey();
                return 1;
            }

           
            // Парсим командную строку
            List<Option> options = ParseArguments(args);             

            if (options.Any())
            {
                ProcessOptions(options);
            }            

            Console.WriteLine("Нажмите любую клавишу для завершения");
            Console.ReadKey();
            return 0;
        }

        private static string CheckConnection(IService service)
        {
            bool resultConnections;
            try
            {
                resultConnections = service.Connection();
            }
            catch (UriFormatException ex)
            {
                return $"{DateTime.Now}: Попытка подключения к '{service.Name}' неуспешна. " +
                    $"Строка подключения '{service.ConnectionString}' . Ошибка {ex.Message}";
            }
            catch (ArgumentException ex)
            {
                return $"{DateTime.Now}: Попытка подключения к '{service.Name}' неуспешна. " +
                    $"Строка подключения '{service.ConnectionString}'. Ошибка: {ex.Message}";
            }
            catch (SqlException ex)
            {
                return $"{DateTime.Now}: Попытка подключения к '{service.Name}' неуспешна. " +
                    $"Строка подключения '{service.ConnectionString}'. Ошибка: {ex.Message}";
            }

            if (resultConnections)
            {
                return $"{DateTime.Now}: Подключение к '{service.Name}' успешно";
            }
            else
            {
                return $"{DateTime.Now}: Подключение к '{service.Name}' неудалось. " +
                    $"Строка подключения '{service.ConnectionString}'";
            }            
        }

        private static void SaveMessagesToXmlFile(List<string> messages, string path)
        {
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

            using (FileStream file = File.Create(path))
            {
                writer.Serialize(file, messages);
            }
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

        private static void SendEmail(List<string> messages)
        {
            if (messages.Any())
            {
                string email = Properties.Settings.Default.Email;
                Console.WriteLine("======");
                Console.WriteLine($"Email будет отправлен на {email}");

                foreach (string message in messages)
                {
                    Console.WriteLine(message);
                }

                Console.WriteLine("======");
            }
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

        private static void ProcessOptions(List<Option> options)
        {
            foreach (var option in options)
            {
                if (option.Name == "--print")
                {
                    List<string> messages = ReadMessagesFromXmlFile(xmlFilePath);
                    Console.WriteLine("Результаты последней проверки:");
                    ShowMessagesOnScreen(messages);
                }

                if (option.Name == "--uri")
                {
                    string uri = option.Value;
                    SaveUriToSettings(uri);
                }

                if (option.Name == "--email")
                {
                    string email = option.Value;
                    SaveEmailToSettings(email);
                }

                if (option.Name == "--dbstring")
                {
                    string dBString = option.Value;
                    SaveDbStringToSettings(dBString);
                }
            }
        }

        private static void ShowMessagesOnScreen(List<string> messages)
        {
            foreach (var message in messages)
                Console.WriteLine(message);
        }

        private static void SaveDbStringToSettings(string dBString)
        {
            if (IsdBStringValid(dBString))
            {
                Console.WriteLine("Сохарняем dBString в настройки");
                Properties.Settings.Default.DbConnectionString = dBString;
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine("dBString невалидный");
            }
        }

        private static void SaveEmailToSettings(string email)
        {
            if (IsEmailValid(email))
            {
                Console.WriteLine("Сохраняем email в настройки");
                Properties.Settings.Default.Email = email;
                Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine("Email невалидный");
            }
        }

        private static void SaveUriToSettings(string uri)
        {
            if (IsUriValid(uri))
            {
                Console.WriteLine("Сохраняем uri в настройки");
                Properties.Settings.Default.Uri = uri;
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

        private static bool IsEmailValid(string email)
        {
            // TODO добавит проверку email на валидность

            return !string.IsNullOrEmpty(email);
        }

        private static bool IsdBStringValid(string dBString)
        {
            return !string.IsNullOrEmpty(dBString);
        }

        
    }
}
