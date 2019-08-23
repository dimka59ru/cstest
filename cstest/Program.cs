using cstest.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace cstest
{
    class Program
    {
        public static readonly string xmlFilePath = "result.xml";
        static readonly List<IOption> setOfOptions = new List<IOption>();
        static readonly List<IService> services = new List<IService>();

        static int Main(string[] args)
        {
            CreateSetOption(); // Создаем набор опций

            if (args.Length == 0)
            {
                Console.WriteLine("Запущено без параметров");

                // Создаем сервисы, которые будут проверены на доступность   
                CreateServices(); 
                // Проверяем сервисы
                List<string> messages = CheckServices();
                // Сохраняем результаты проверки в файл
                SaveMessagesToXmlFile(messages, xmlFilePath);
                // Отправляем результаты проверки на Email
                SendEmail(messages);

                return 1;
            }

            // Парсим командную строку
            List<OptionReaded> readedOptions = ParseArguments(args);

            if (readedOptions.Any())
            {
                // Обрабаотываем опции
                ProcessOptions(readedOptions);
            }

            Console.WriteLine("Нажмите любую клавишу для завершения");
            Console.ReadKey();
            return 0;
        }

        private static List<string> CheckServices()
        {
            List<string> messages = new List<string>();

            foreach (var service in services)
            {
                string result = CheckConnection(service);
                messages.Add(result);
            }
            return messages;
        }

        private static void CreateServices()
        {
            SiteService site = new SiteService
            {
                Name = $"Сайт {Properties.Settings.Default.Uri}",
                ConnectionString = Properties.Settings.Default.Uri
            };
            services.Add(site);

            DBService dB = new DBService
            {
                Name = "БД MSSQL",
                ConnectionString = Properties.Settings.Default.DbConnectionString
            };
            services.Add(dB);
        }

        private static void CreateSetOption()
        {
            SaveEmailOption emailOption = new SaveEmailOption();
            emailOption.Key = "--email";
            setOfOptions.Add(emailOption);

            SaveDbStringOption dbStringOption = new SaveDbStringOption();
            dbStringOption.Key = "--dbstring";
            setOfOptions.Add(dbStringOption);

            SaveUriOption uriOption = new SaveUriOption();
            uriOption.Key = "--uri";
            setOfOptions.Add(uriOption);

            ShowLastResultOption lastResultOption = new ShowLastResultOption();
            lastResultOption.Key = "--print";
            setOfOptions.Add(lastResultOption);
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

        private static List<OptionReaded> ParseArguments(string[] args)
        {
            bool argWasRead = true;

            List<OptionReaded> options = new List<OptionReaded>();
            OptionReaded option = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--")) // Если аргумент начинается с --
                {
                    option = new OptionReaded();
                    option.Key = arg;
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

        private static void ProcessOptions(List<OptionReaded> ReadedOptions)
        {
            foreach (var readedOption in ReadedOptions)
            {
                // Проверяем, есть ли введеная опция, среди набора опций
                var opt = setOfOptions.FirstOrDefault(x => x.Key == readedOption.Key);
                if (opt != null)
                {
                    opt.Value = readedOption.Value; // Задаем значение опции
                    opt.Action();                   // Выполняем действие
                }
                else
                {
                    Console.WriteLine($"Опция {readedOption.Key} не найдена");
                }                
            }
        }        
    }
}
