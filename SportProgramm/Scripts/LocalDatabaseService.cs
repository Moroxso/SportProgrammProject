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
                // Используем стандартную строку подключения
                _context = new SportProgrammProjectEntities();

                if (TestConnectionWithRetry())
                {
                    IsConnected = true;
                    // ТИХОЕ подключение - без MessageBox
                    System.Diagnostics.Debug.WriteLine("Успешное подключение к LocalDB");
                    return;
                }

                CreateDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации LocalDB: {ex.Message}");
                IsConnected = false;
            }
        }

        private bool TestConnectionWithRetry()
        {
            string[] connectionStrings = {
            @"metadata=res://*/BaseDate.Model1.csdl|res://*/BaseDate.Model1.ssdl|res://*/BaseDate.Model1.msl;provider=System.Data.SqlClient;provider connection string=""data source=(localdb)\MSSQLLocalDB;initial catalog=SportProgrammProject;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""",
            @"data source=(localdb)\MSSQLLocalDB;initial catalog=SportProgrammProject;integrated security=True;MultipleActiveResultSets=True",
        };

            foreach (string connString in connectionStrings)
            {
                try
                {
                    _context = new SportProgrammProjectEntities();
                    _context.Database.Connection.ConnectionString = connString;

                    if (_context.Database.Exists() && CheckDatabaseStructure())
                    {
                        return true;
                    }
                }
                catch
                {
                    _context?.Dispose();
                    _context = null;
                }
            }
            return false;
        }

        private bool CheckDatabaseStructure()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SELECT TOP 1 1 FROM Sports");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateDatabase()
        {
            try
            {
                string connectionString = @"data source=(localdb)\MSSQLLocalDB;initial catalog=SportProgrammProject;integrated security=True;MultipleActiveResultSets=True";

                _context = new SportProgrammProjectEntities();
                _context.Database.Connection.ConnectionString = connectionString;

                if (!_context.Database.Exists())
                {
                    _context.Database.Create();
                    CreateTablesManually();
                    SeedInitialData();
                }

                IsConnected = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания базы: {ex.Message}");
                IsConnected = false;
            }
        }

        private void CreateTablesManually()
        {
            try
            {
                // SQL скрипты создания таблиц (без MessageBox)
                string[] createScripts = {
                @"CREATE TABLE Sports(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(100) NOT NULL, Unit NVARCHAR(50), Record NVARCHAR(100), Date DATETIME)",
                @"CREATE TABLE Roles(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(50) NOT NULL)",
                @"CREATE TABLE Users(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(100) NOT NULL, Login NVARCHAR(50) NOT NULL UNIQUE, Password NVARCHAR(100) NOT NULL, IdRole INT NOT NULL)",
                @"CREATE TABLE Sportman(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(100) NOT NULL, Date DATETIME, Team NVARCHAR(100), Lvl NVARCHAR(50), IdSport_1 INT NOT NULL, IdSport_2 INT, IdSport_3 INT, IdSport_4 INT, IdSport_5 INT)",
                @"CREATE TABLE Cup(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(100) NOT NULL, Score NVARCHAR(100), Date DATETIME NOT NULL, Place NVARCHAR(100), IdSport INT NOT NULL)",
                @"CREATE TABLE TournamentResults(Id INT PRIMARY KEY IDENTITY, IdCup INT NOT NULL, IdPlayer INT NOT NULL, Position INT, Score NVARCHAR(50), Points INT)"
            };

                foreach (var script in createScripts)
                {
                    try
                    {
                        _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, script);
                    }
                    catch { /* Игнорируем ошибки */ }
                }
            }
            catch { /* Игнорируем ошибки */ }
        }

        private void SeedInitialData()
        {
            try
            {
                if (!_context.Roles.Any())
                {
                    _context.Roles.Add(new Roles { Name = "User" });
                    _context.Roles.Add(new Roles { Name = "Admin" });
                    _context.SaveChanges();
                }

                if (!_context.Users.Any(u => u.Login == "admin"))
                {
                    var adminRole = _context.Roles.First(r => r.Name == "Admin");
                    _context.Users.Add(new Users { Name = "Администратор", Login = "admin", Password = "admin123", IdRole = adminRole.Id });
                    _context.SaveChanges();
                }

                if (!_context.Sports.Any())
                {
                    _context.Sports.Add(new Sports { Name = "Шахматы", Unit = "очки", Date = DateTime.Now });
                    _context.Sports.Add(new Sports { Name = "Футбол", Unit = "голы", Date = DateTime.Now });
                    _context.Sports.Add(new Sports { Name = "Баскетбол", Unit = "очки", Date = DateTime.Now });
                    _context.SaveChanges();
                }
            }
            catch { /* Игнорируем ошибки */ }
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



