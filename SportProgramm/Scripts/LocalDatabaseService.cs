using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Data.SqlClient;

namespace SportProgramm.Scripts
{
    public class LocalDatabaseService : IDatabaseService
    {
        private SportProgrammProjectEntities _context;
        public bool IsConnected { get; private set; }
        public SportProgrammProjectEntities Context => _context;

        public void Initialize()
        {
            try
            {
                // Пробуем разные варианты подключения
                if (TryConnectToExistingDatabase())
                {
                    IsConnected = true;
                    return;
                }

                // Если не удалось подключиться к существующей, пробуем создать новую
                if (TryCreateNewDatabase())
                {
                    IsConnected = true;
                    return;
                }

                // Если ничего не помогло
                IsConnected = false;
                ShowConnectionError("Не удалось подключиться к базе данных");
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ShowConnectionError($"Ошибка: {ex.Message}");
            }
        }

        private bool TryConnectToExistingDatabase()
        {
            // Пробуем разные строки подключения к существующим базам
            string[] connectionStrings = {
                // Подключение к существующей базе SportProgrammProject
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=SportProgrammProject;Integrated Security=True;Connect Timeout=30",
                // SQL Server Express
                @"Data Source=.\SQLEXPRESS;Initial Catalog=SportProgrammProject;Integrated Security=True;Connect Timeout=30",
                // Local SQL Server
                @"Data Source=(local);Initial Catalog=SportProgrammProject;Integrated Security=True;Connect Timeout=30",
                // Имя сервера
                @"Data Source=localhost;Initial Catalog=SportProgrammProject;Integrated Security=True;Connect Timeout=30"
            };

            foreach (string connString in connectionStrings)
            {
                try
                {
                    _context = new SportProgrammProjectEntities();
                    _context.Database.Connection.ConnectionString = connString;

                    // Быстрая проверка подключения
                    _context.Database.Connection.Open();
                    _context.Database.Connection.Close();

                    // Проверяем, что основные таблицы существуют
                    if (CheckDatabaseStructure())
                    {
                        System.Diagnostics.Debug.WriteLine($"Успешное подключение к существующей базе: {connString}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Не удалось подключиться {connString}: {ex.Message}");
                    _context?.Dispose();
                    _context = null;
                }
            }

            return false;
        }

        private bool TryCreateNewDatabase()
        {
            try
            {
                // Проверяем, установлен ли LocalDB
                if (!IsLocalDBAvailable())
                {
                    ShowLocalDBInstallationGuide();
                    return false;
                }

                MessageBox.Show("Создание новой базы данных...");

                // Создаем базу через master
                using (var masterConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30"))
                {
                    masterConnection.Open();

                    // Создаем базу если не существует
                    string createDbCommand = @"
                        IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'SportProgrammProject')
                        BEGIN
                            CREATE DATABASE [SportProgrammProject]
                        END";

                    using (var command = new SqlCommand(createDbCommand, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }

                    masterConnection.Close();
                }

                // Подключаемся к созданной/существующей базе
                _context = new SportProgrammProjectEntities();
                _context.Database.Connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=SportProgrammProject;Integrated Security=True;Connect Timeout=30";

                _context.Database.Connection.Open();
                _context.Database.Connection.Close();

                // Создаем таблицы если их нет
                CreateTablesIfNotExist();

                // Заполняем начальными данными если нужно
                SeedInitialDataIfEmpty();

                System.Diagnostics.Debug.WriteLine("База данных успешно создана/подключена");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания базы: {ex.Message}");
                return false;
            }
        }

        private bool IsLocalDBAvailable()
        {
            try
            {
                using (var connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=5"))
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool CheckDatabaseStructure()
        {
            try
            {
                // Проверяем существование основных таблиц
                var requiredTables = new[] { "Sports", "Roles", "Users", "Sportman", "Cup" };

                foreach (var table in requiredTables)
                {
                    var checkTableCommand = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table}'";
                    var result = _context.Database.SqlQuery<int>(checkTableCommand).First();

                    if (result == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Таблица {table} не найдена");
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateTablesIfNotExist()
        {
            try
            {
                // Проверяем и создаем таблицы если их нет
                string[] tableCreationScripts = {
                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Sports')
                    CREATE TABLE [Sports] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(100) NOT NULL,
                        [Unit] NVARCHAR(50) NULL,
                        [Record] NVARCHAR(100) NULL,
                        [Date] DATETIME NULL
                    )",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
                    CREATE TABLE [Roles] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(50) NOT NULL
                    )",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
                    CREATE TABLE [Users] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(100) NOT NULL,
                        [Login] NVARCHAR(50) NOT NULL UNIQUE,
                        [Password] NVARCHAR(100) NOT NULL,
                        [IdRole] INT NOT NULL
                    )",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Sportman')
                    CREATE TABLE [Sportman] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(100) NOT NULL,
                        [Date] DATETIME NULL,
                        [Team] NVARCHAR(100) NULL,
                        [Lvl] NVARCHAR(50) NULL,
                        [IdSport_1] INT NOT NULL,
                        [IdSport_2] INT NULL,
                        [IdSport_3] INT NULL,
                        [IdSport_4] INT NULL,
                        [IdSport_5] INT NULL
                    )",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Cup')
                    CREATE TABLE [Cup] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [Name] NVARCHAR(100) NOT NULL,
                        [Score] NVARCHAR(100) NULL,
                        [Date] DATETIME NOT NULL,
                        [Place] NVARCHAR(100) NULL,
                        [IdSport] INT NOT NULL
                    )",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TournamentResults')
                    CREATE TABLE [TournamentResults] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [IdCup] INT NOT NULL,
                        [IdPlayer] INT NOT NULL,
                        [Position] INT NULL,
                        [Score] NVARCHAR(50) NULL,
                        [Points] INT NULL
                    )"
                };

                foreach (var script in tableCreationScripts)
                {
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script);
                }

                // Создаем внешние ключи если их нет
                string[] foreignKeyScripts = {
                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Users_Roles')
                    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([IdRole]) REFERENCES [Roles]([Id])",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Sportman_Sports')
                    ALTER TABLE [Sportman] ADD CONSTRAINT [FK_Sportman_Sports] FOREIGN KEY ([IdSport_1]) REFERENCES [Sports]([Id])",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Cup_Sports')
                    ALTER TABLE [Cup] ADD CONSTRAINT [FK_Cup_Sports] FOREIGN KEY ([IdSport]) REFERENCES [Sports]([Id])",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_TournamentResults_Cup')
                    ALTER TABLE [TournamentResults] ADD CONSTRAINT [FK_TournamentResults_Cup] FOREIGN KEY ([IdCup]) REFERENCES [Cup]([Id])",

                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_TournamentResults_Sportman')
                    ALTER TABLE [TournamentResults] ADD CONSTRAINT [FK_TournamentResults_Sportman] FOREIGN KEY ([IdPlayer]) REFERENCES [Sportman]([Id])"
                };

                foreach (var script in foreignKeyScripts)
                {
                    _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания таблиц: {ex.Message}");
            }
        }

        private void SeedInitialDataIfEmpty()
        {
            try
            {
                // Добавляем роли если их нет
                if (!_context.Roles.Any())
                {
                    _context.Roles.Add(new Roles { Name = "Admin" });
                    _context.Roles.Add(new Roles { Name = "User" });
                    _context.SaveChanges();
                }

                // Добавляем администратора если нет пользователей
                if (!_context.Users.Any(u => u.Login == "admin"))
                {
                    var adminRole = _context.Roles.First(r => r.Name == "Admin");
                    _context.Users.Add(new Users
                    {
                        Name = "Администратор",
                        Login = "admin",
                        Password = "12345",
                        IdRole = adminRole.Id
                    });
                    _context.SaveChanges();
                }

                // Добавляем виды спорта если их нет
                if (!_context.Sports.Any())
                {
                    _context.Sports.Add(new Sports { Name = "Шахматы", Unit = "очки", Date = DateTime.Now });
                    _context.Sports.Add(new Sports { Name = "Футбол", Unit = "голы", Date = DateTime.Now });
                    _context.Sports.Add(new Sports { Name = "Баскетбол", Unit = "очки", Date = DateTime.Now });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка начальных данных: {ex.Message}");
            }
        }

        private void ShowLocalDBInstallationGuide()
        {
            MessageBox.Show(@"Для работы приложения требуется SQL Server LocalDB.

Как установить:
1. Скачайте SQL Server Express с официального сайта Microsoft
2. При установке выберите 'LocalDB'
3. Или установите через Visual Studio Installer

Приложение будет работать после установки LocalDB.",
"Требуется LocalDB",
MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowConnectionError(string message)
        {
            MessageBox.Show($@"{message}

Проверьте:
1. Установлен ли SQL Server LocalDB
2. Не запущено ли другое приложение, использующее базу данных
3. Доступны ли права на создание базы данных",
"Ошибка подключения",
MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void TestConnection()
        {
            IsConnected = _context?.Database?.Exists() ?? false;
        }

        public void CreateBackup()
        {
            MessageBox.Show("Функция бэкапа не реализована");
        }

        public void RestoreFromBackup(string backupPath)
        {
            MessageBox.Show("Функция восстановления не реализована");
        }
    }
}





