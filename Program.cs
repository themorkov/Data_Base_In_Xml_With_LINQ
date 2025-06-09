using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
namespace Data_Base_In_Xml_With_LINQ
{
    class Program
    {
        static void Main()
        {
            WorkWithXml xmlFile = new("DB of companies.xml");
            Console.WriteLine("Добро пожаловать в приложение для поиска работы!");
            if(xmlFile.HasCompanies())
            {
                Console.WriteLine("Для входа в личный кабинет накжмите Enter.");
                Console.WriteLine("Для внесения организации в базу нажмите пробел.");
                Console.WriteLine("Для просмотра базы вакансий нажмите любую клавишу.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Введите логин(название организации:)");
                    string? login = Console.ReadLine();
                    string? password;
                    Console.WriteLine("Введите пароль:");
                    password = Console.ReadLine();
                    if (xmlFile.CheckLogAndPass(login, password))
                    {
                        Console.WriteLine("Для изменения информации о организации нажмите 1:");
                        Console.WriteLine("Для удаления организации нажмите 2:");
                        Console.WriteLine();
                       key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.D1)
                        {
                            xmlFile.ChangeCompany(login, password);
                        }
                        if (key.Key == ConsoleKey.D2)
                        {
                            Console.WriteLine("Введите пароль и нажмите Enter для удаления файла организации из базы.");
                            string? pass = Console.ReadLine();
                            if (pass == password)
                            {
                                xmlFile.DeleteCompany(login, password);
                            }
                            else
                            {
                                Console.WriteLine("Неверный пароль.");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Перезапустите программу для повторной попытки входа в личный кабинет.");
                    }
                }
                if (key.Key == ConsoleKey.Spacebar)
                {
                    xmlFile.CreateNewCompany();
                }
                else
                {
                    bool work = false;
                    while (!work)
                    {

                        Console.WriteLine("Для просмотра списка организаций нажмите: 1");
                        Console.WriteLine("Для просмотра списка вакансий нажмите: 2");
                        Console.WriteLine("Для поиска вакансий по должности нажмите: 3");
                        Console.WriteLine("Для выхода из программы нажмите: Esc");
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.D1)
                        {
                            xmlFile.ShowCompanies();
                            Console.WriteLine("Для просмотра организации нажмите: 1, для выхода любую клавишу.");
                            key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.D1)
                            {
                                Console.WriteLine("Введите название организации:");
                                string? company = Console.ReadLine();
                                if (xmlFile.ShowCompany(company))
                                {
                                    Console.WriteLine("Введите имя вакансии для просмотра:");
                                    string? position = Console.ReadLine();
                                    xmlFile.ShowVacancy(position, company);

                                }
                            }
                        }
                        if (key.Key == ConsoleKey.D2)
                        {
                            if (xmlFile.ShowVacancies())
                            {
                                Console.WriteLine("Введите имя вакансии для просмотра:");
                                string? position = Console.ReadLine();
                                xmlFile.ShowVacancy(position);
                            }
                        }
                        if (key.Key == ConsoleKey.D3)
                        {
                            Console.WriteLine("Введите имя вакансии для просмотра:");
                            string? position = Console.ReadLine();
                            xmlFile.ShowVacancy(position);
                        }
                        if (key.Key == ConsoleKey.Escape)
                        {
                            System.Environment.Exit(0);//системный метод, закрывает консоль
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine("База организаций пуста.");
                Console.WriteLine("Для внесения организации в базу нажмите пробел.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Spacebar)
                {
                    if(xmlFile.CreateNewCompany())
                    {
                        Console.WriteLine("Для использования базы, перезагрузите приложение.");
                    }
                }
            }
        }
    }
}
