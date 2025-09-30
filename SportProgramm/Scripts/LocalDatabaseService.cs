using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportProgramm.Scripts
{
    public class LocalDatabaseService : IDatabaseService
    {
        private SportProgrammProjectEntities _context;
        public bool IsConnected { get; private set; }
        private string _localDbPath;

        public void Initialize()
        {
            try
            {
                _localDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "LocalSportDB.mdf");
                Directory.CreateDirectory(Path.GetDirectoryName(_localDbPath));

                // Создаем строку подключения для LocalDB
                string localConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={_localDbPath};Integrated Security=True;Connect Timeout=30";

                _context = new SportProgrammProjectEntities(localConnectionString);

                // Создаем базу если не существует
                CreateDatabaseIfNotExists();
                TestConnection();
            }
            catch
            {
                IsConnected = false;
            }
        }

        private void CreateDatabaseIfNotExists()
        {
            if (!File.Exists(_localDbPath))
            {
                try
                {
                    // Создаем базу данных
                    string createScript = $@"CREATE DATABASE [LocalSportDB] ON (NAME = N'LocalSportDB', FILENAME = '{_localDbPath}')";

                    using (var masterContext = new SportProgrammProjectEntities(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"))
                    {
                        masterContext.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, createScript);
                    }

                    // Создаем таблицы
                    CreateTables();
                    SeedInitialData();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка создания локальной базы: {ex.Message}");
                }
            }
        }

        private void CreateTables()
        {
            // SQL скрипты для создания таблиц
            string[] createTableScripts = {
            @"CREATE TABLE Sports (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100) NOT NULL,
                Unit NVARCHAR(50),
                Record NVARCHAR(100),
                Date DATETIME
            )",

            @"CREATE TABLE Roles (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(50) NOT NULL
            )",

            @"CREATE TABLE Users (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100) NOT NULL,
                Login NVARCHAR(50) NOT NULL UNIQUE,
                Password NVARCHAR(100) NOT NULL,
                IdRole INT NOT NULL,
                FOREIGN KEY (IdRole) REFERENCES Roles(Id)
            )",

            @"CREATE TABLE Sportman (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100) NOT NULL,
                Date DATETIME,
                Team NVARCHAR(100),
                Lvl NVARCHAR(50),
                IdSport_1 INT NOT NULL,
                IdSport_2 INT,
                IdSport_3 INT,
                IdSport_4 INT,
                IdSport_5 INT,
                FOREIGN KEY (IdSport_1) REFERENCES Sports(Id)
            )",

            @"CREATE TABLE Cup (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100) NOT NULL,
                Score NVARCHAR(100),
                Date DATETIME NOT NULL,
                Place NVARCHAR(100),
                IdSport INT NOT NULL,
                FOREIGN KEY (IdSport) REFERENCES Sports(Id)
            )",

            @"CREATE TABLE TournamentResults (
                Id INT PRIMARY KEY IDENTITY,
                IdCup INT NOT NULL,
                IdPlayer INT NOT NULL,
                Position INT,
                Score NVARCHAR(50),
                Points INT,
                FOREIGN KEY (IdCup) REFERENCES Cup(Id),
                FOREIGN KEY (IdPlayer) REFERENCES Sportman(Id)
            )"
        };

            foreach (var script in createTableScripts)
            {
                _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script);
            }
        }

        private void SeedInitialData()
        {
            // Добавляем начальные данные
            try
            {
                // Роли
                _context.Roles.Add(new Roles { Name = "User" });
                _context.Roles.Add(new Roles { Name = "Admin" });
                _context.SaveChanges();

                // Администратор по умолчанию
                var adminRole = _context.Roles.First(r => r.Name == "Admin");
                _context.Users.Add(new Users
                {
                    Name = "Администратор",
                    Login = "admin",
                    Password = "admin123",
                    IdRole = adminRole.Id
                });

                // Примерные виды спорта
                _context.Sports.Add(new Sports { Name = "Шахматы", Unit = "очки", Date = DateTime.Now });
                _context.Sports.Add(new Sports { Name = "Футбол", Unit = "голы", Date = DateTime.Now });
                _context.Sports.Add(new Sports { Name = "Баскетбол", Unit = "очки", Date = DateTime.Now });

                _context.SaveChanges();
            }
            catch
            {
                // Игнорируем ошибки при добавлении начальных данных
            }
        }

        public void TestConnection()
        {
            try
            {
                IsConnected = _context.Database.Exists();
            }
            catch
            {
                IsConnected = false;
            }
        }

        public void CreateBackup()
        {
            // Для LocalDB просто копируем файл
            try
            {
                string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                Directory.CreateDirectory(backupDir);

                string backupFile = Path.Combine(backupDir, $"LocalBackup_{DateTime.Now:yyyyMMdd_HHmmss}.mdf");
                File.Copy(_localDbPath, backupFile, true);

                // Копируем лог файл если существует
                string logFile = _localDbPath.Replace(".mdf", "_log.ldf");
                if (File.Exists(logFile))
                {
                    string backupLogFile = Path.Combine(backupDir, $"LocalBackup_{DateTime.Now:yyyyMMdd_HHmmss}.ldf");
                    File.Copy(logFile, backupLogFile, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания бэкапа: {ex.Message}");
            }
        }

        public void RestoreFromBackup(string backupPath)
        {
            try
            {
                // Останавливаем подключения к базе
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

                // Копируем файл бэкапа
                File.Copy(backupPath, _localDbPath, true);

                // Переинициализируем контекст
                Initialize();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка восстановления: {ex.Message}");
            }
        }

        public SportProgrammProjectEntities GetContext()
        {
            return _context;
        }
    }
}
