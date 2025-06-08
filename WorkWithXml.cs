using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data_Base_In_Xml_With_LINQ
{
    internal class WorkWithXml
    {
        /// <value>
        /// Поле <c>xmlDocument</c> это элемент класса XDocument
        /// в пространстве имён System.Xml.Linq,
        /// который представляет весь xml-документ.
        /// </value>
        XDocument xmlDocument;
        /// <value>
        /// Поле <c>companies</c> это элемент класса XElement
        /// в пространстве имён System.Xml.Linq,
        /// в котором содержатся дочерние элементы организаций.
        /// </value>
        XElement? companies;
        ///<summary>
        ///Данный конструктор проверяет наличие файла по адресу.
        ///Если, файл существует, то инициализирует экземпляр класса <c>WorkWithXml</c>.
        ///Если, нет то предлагает создать файл с дальнейшей инициализацией.
        ///</summary>
        ///<param name="adress"> Содержит путь к XML файлу.</param>
        public WorkWithXml(string adress)
        {
            if (File.Exists(adress))
            {
                xmlDocument = XDocument.Load(adress);
            }
            else
            {
                Console.WriteLine("Файл базы данных не обнаружен");
                Console.WriteLine("Для создания файла нажмите 1.");
                Console.WriteLine("Для выхода нажмите любую клавишу.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.D1)
                {
                    xmlDocument = new XDocument(new XElement("companies"));
                    xmlDocument.Save(adress);
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }

            companies = xmlDocument.Element("companies");
        }
        /// <summary>
        /// Данный метод проверяет наличие файлов организаций в базе.
        /// </summary>
        /// <returns>Если организации найдены, возвращает истину, если нет то ложь.</returns>
        public bool HasCompanies()
        {
            if(companies.HasElements)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Данный метод выводит в консоль имена всех организаций.
        /// </summary>
        public void ShowCompanies()
        {
            var companyNames = companies.Elements("company")?
                 .Select(c => c.Attribute("name")?.Value);
            if (companyNames != null && companyNames.Any())
            {
                Console.WriteLine("Список организаций:");
                foreach (var name in companyNames)
                {
                    Console.WriteLine($"Организация: {name}");
                }
            }
            else
            {
                Console.WriteLine("Список организаций пуст.");
            }
        }
        /// <summary>
        /// Метод аутентификации пользователя.
        /// Сверяет полученные от пользователя логин и пароль.
        /// </summary>
        /// <returns>Возвращает истину если, логин и пароль совпали.</returns>
        /// <param name="login">Полученный логин от пользователя(название организации).</param>
        /// <param name="password">Полученный пароль от пользователя.</param>
        public bool CheckLogAndPass(string? login, string? password)
        {
            var companyLoginAndPassword = companies.Elements("company")
                .FirstOrDefault(c => (string?)c.Attribute("name") == login &&
                (string?)c.Attribute("password") == password);
            if ((companyLoginAndPassword != null))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Неправильный логин или пароль.");
                return false;
            }
        }
        /// <summary>
        /// Метод выводит в консоль:
        ///название организации, город, телефон и доступные вакансии.
        /// </summary>
        /// <returns>Возвращает истину если организация найдена в базе.</returns>
        /// <param name="companyName">Название организации</param>
        public bool ShowCompany(string companyName)
        {

            var company = companies.Elements("company")?
                .FirstOrDefault(c => (string?)
                c.Attribute("name") == companyName);

            if (company == null)
            {
                Console.WriteLine("Компания не найдена.");
                return false;
            }

            string? name = (string?)company.Attribute("name");
            string? city = (string?)company.Element("city");
            string? phone = (string?)company.Element("phone");

            Console.WriteLine($"Организация: {name}");
            Console.WriteLine($"Город: {city}");
            Console.WriteLine($"Телефон: {phone}");
            Console.WriteLine($"Доступные вакансии в {name}:");

            var vacancies = company.Element("vacancies")?
                .Elements("vacancy")
                .Select(v => (string?)v.Element("position"))
                .ToList();

            if (vacancies != null && vacancies.Any())
            {
                foreach (var position in vacancies)
                {
                    Console.WriteLine(position);
                }
                return true;
            }
            else
            {
                Console.WriteLine("Вакансий нет.");
                return true;
            }
        }
        /// <summary>
        /// Метод выводит на консоль все доступные вакансии из базы.
        /// </summary>
        /// <returns>Возращает истину, если в базе есть вакансии.</returns>
        public bool ShowVacancies()
        {
            var vacancies = companies.Elements("company")?
                .SelectMany(c => c.Element("vacancies")
                .Elements("vacancy"))
                .Select(v => v.Element("position")?.Value)
                .ToList();
            if (vacancies != null && vacancies.Any())
            {
                foreach (var position in vacancies)
                {
                    Console.WriteLine($"Вакансия: {position}");
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Метод выводит на консоль все данные о запрашиваемой вакансии:
        /// Должность, требуемый опыт работы и размер зарплаты.
        /// </summary>
        /// <param name="positionName">Имя вакансии</param>
        public void ShowVacancy(string positionName)
        {
            var allVacancies = companies.Elements("company")?
                .SelectMany(company =>
                    company.Element("vacancies")?.Elements("vacancy")
                    .Select(vacancy => new {
                        CompanyName = (string?)company.Attribute("name"),
                        Vacancy = vacancy
                    }) ?? Enumerable.Empty<dynamic>())
                .Where(x => (string?)x.Vacancy.Element("position") == positionName)
                .ToList();

            if (allVacancies.Any())
            {
                foreach (var item in allVacancies)
                {
                    Console.WriteLine($"Вакансия найдена в организации: {item.CompanyName}");
                    Console.WriteLine($"Должность: {item.Vacancy.Element("position")?.Value}");
                    Console.WriteLine($"Опыт работы: {item.Vacancy.Element("experience")?.Value}");
                    Console.WriteLine($"Зарплата: {item.Vacancy.Element("salary")?.Value}");
                }
            }
            else
            {
                Console.WriteLine("Вакансия не найдена.");
            }
        }
        /// <summary>
        /// Перегруженный метод показа вакансии для конкретной организации.
        /// Выводит на консоль: должность, требуемый опыт работы и размер зарплаты.
        /// </summary>
        /// <param name="positionName">Имя вакансии.</param>
        /// <param name="companyName">Название организации.</param>
        public void ShowVacancy(string positionName, string companyName)
        {

            var vacancies = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName)
                ?.Element("vacancies");
            if (vacancies is not null)
            {
                foreach (XElement vacancy in vacancies.Elements("vacancy"))
                {
                    XElement? position = vacancy.Element("position");
                    if (position?.Value == positionName)
                    {
                        XElement? experience = vacancy.Element("experience");
                        XElement? salary = vacancy.Element("salary");

                        Console.WriteLine($"Вакансия для организации: {companyName}");
                        Console.WriteLine($"Должность: {position?.Value}");
                        Console.WriteLine($"Опыт работы: {experience?.Value}");
                        Console.WriteLine($"Зарплата: {salary?.Value}");
                        return;
                    }
                }
            }
            Console.WriteLine("Вакансия не найдена.");
        }
        /// <summary>
        /// Метод создания файла для новой организации.
        /// </summary>
        /// <returns>Возвращает истину после выполнения.</returns>
        public bool CreateNewCompany()
        {
            Console.WriteLine("Введите название организации(оно же логин для входа):");
            string? name = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            string? password = Console.ReadLine();
            Console.WriteLine("Введите город расположения:");
            string? city = Console.ReadLine();
            Console.WriteLine("Введите телефон организации:");
            string? phone = Console.ReadLine();
            companies.Add(new XElement("company",
                new XAttribute("name", name),
                new XAttribute("password", password),
                new XElement("city", city),
                new XElement("phone", phone),
                new XElement("vacancies")));
            xmlDocument.Save("companies2.xml");
            Console.WriteLine($"Файл компании {name} был успешно создан!");
            return true;
        }
        /// <summary>
        /// Приватный метод добавления новой вакансии для организации.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Пароль организации,
        /// на случай нескольких организаций с одинаковыми именами.</param>
        /// <returns>Возвращает истину, если вакансия успешно добавлена.</returns>
        private bool AddVacancy(string companyName, string companyPassword)
        {
            var vacancies = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName &&
                (string?)c.Attribute("password") == companyPassword)
                ?.Element("vacancies");
            if (vacancies != null)
            {
                Console.WriteLine("Введите должность:");
                string? position = Console.ReadLine();
                Console.WriteLine("Введите требуемый стаж:");
                string? experience = Console.ReadLine();
                Console.WriteLine("Введите зарплату:");
                string? salary = Console.ReadLine();
                vacancies.Add(new XElement("vacancy",
                            new XElement("position", position),
                            new XElement("experience", experience),
                            new XElement("salary", salary)));
                xmlDocument.Save("companies2.xml");
                Console.WriteLine($"Вакансия {position} была добавлена!");
                return true;
            }
            return false;
        }
        /// <summary>
        /// Приватный метод изменения номера телефона для конкретной организации.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Пароль для конкретной организации.</param>
        /// <returns>Возвращает истину при замене номера телефона.</returns>
        private bool ChangePhone(string companyName, string companyPassword)
        {
            Console.WriteLine("Введите новый номер телефона:");
            string? newPhone = Console.ReadLine();
            var company = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName &&
                (string?)c.Attribute("password") == companyPassword);
            if (newPhone != null && company != null)
            {
                XElement? phone = company.Element("phone");
                Console.WriteLine("Изменения внесены.");
                Console.WriteLine($"Старый номер организации: {phone?.Value}");
                phone.Value = newPhone;
                Console.WriteLine($"Новый номер организации: {phone?.Value}");
                xmlDocument.Save("companies2.xml");
                return true;
            }
            else
            {
                Console.WriteLine("Ошибка ввода");
                return false;
            }
        }
        /// <summary>
        /// Приватный метод изменения информации о вакансии.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Парол для организации.</param>
        /// <returns>Возвращает истину, если вакансия была изменена.</returns>
        private bool ChangeVacancy(string companyName, string companyPassword)
        {
            Console.WriteLine("Введите имя вакансии, которую хотите изменить:");
            string? newVacancy = Console.ReadLine();
            var vacancies = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName &&
                (string?)c.Attribute("password") == companyPassword)
                ?.Element("vacancies");
            if (vacancies is not null)
            {
                foreach (XElement? vacancy in vacancies.Elements("vacancy"))
                {
                    XElement? position = vacancy.Element("position");
                    if (position?.Value == newVacancy)
                    {
                        Console.WriteLine("Введите новое значение требуемого стажа:");
                        string? newExperience = Console.ReadLine();
                        Console.WriteLine("Введите новое значение зарплаты:");
                        string? newSalary = Console.ReadLine();
                        XElement? experience = vacancy.Element("experience");
                        XElement? salary = vacancy.Element("salary");
                        experience.Value = newExperience;
                        salary.Value = newSalary;
                        Console.WriteLine($"Изменения для вакансии {position.Value} успешно сохранены.");
                        xmlDocument.Save("companies2.xml");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Вакансия {newVacancy} остутствует в базе.");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("База вакансий пуста");
                return false;
            }
            return false;
            
        }
        /// <summary>
        /// Приватный метод удаления вакансии из базы.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Пароль для организации.</param>
        /// <returns>Возвращает истину, при успешном удалении вакансии.</returns>
        private bool DeleteVacancy(string companyName,string companyPassword)
        {
            Console.WriteLine("Введите имя вакансии, которую хотите удалить:");
            string? markedVacancy = Console.ReadLine();
            var vacancies = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName &&
                (string?)c.Attribute("password") == companyPassword)
                ?.Element("vacancies");
            if (vacancies is not null)
            {
                foreach (XElement? vacancy in vacancies.Elements("vacancy"))
                {
                    XElement? position = vacancy.Element("position");
                    if (position?.Value == markedVacancy)
                    {
                        vacancy.Remove();
                        Console.WriteLine($"Вакансия {position.Value} была удалена.");
                        xmlDocument.Save("companies2.xml");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Вакансия {markedVacancy} остутствует в базе.");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("База вакансий пуста");
                return false;
            }
            return false;
        }
        /// <summary>
        /// Метод изменения информации для конкретной организации.
        /// Включает приватные методы:
        /// Добавления вакансии, изменения номера телефона,  изменения и удаления вакансии.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Пароль для организации.</param>
        public void ChangeCompany(string companyName, string companyPassword)
        {
            Console.WriteLine("Для добавления вакансии нажмите 1:");
            Console.WriteLine("Для изменения номера телефона нажмите 2:");
            Console.WriteLine("Для изменения вакансии нажмите 3:");
            Console.WriteLine("Для удаления вакансии нажмите 4:");
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.D1)
            {
                AddVacancy(companyName, companyPassword);
            }
            if (key.Key == ConsoleKey.D2)
            {
                ChangePhone(companyName, companyPassword);
            }
            if (key.Key == ConsoleKey.D3)
            {
                ChangeVacancy(companyName, companyPassword);
            }
            if (key.Key == ConsoleKey.D4)
            {
                DeleteVacancy(companyName, companyPassword);
            }
        }
        /// <summary>
        /// Метод удаления организации из базы.
        /// </summary>
        /// <param name="companyName">Название организации.</param>
        /// <param name="companyPassword">Пароль для организации.</param>
        /// <returns>Возвращяет истину, если организация была удалена из базы.</returns>
        public bool DeleteCompany(string companyName, string companyPassword)
        {
            var company = companies.Elements("company")?
                .FirstOrDefault(c => (string?)c.Attribute("name") == companyName &&
                (string?)c.Attribute("password") == companyPassword);
            if (company != null)
            {
                company.Remove();
                xmlDocument.Save("companies2.xml");
                Console.WriteLine($"Организация {companyName} была удалена из базы.");
                return true;
            }
            else
            {
                Console.WriteLine($"Организация {companyName} не найдена.");
                return false;
            }

        }
    }
}
