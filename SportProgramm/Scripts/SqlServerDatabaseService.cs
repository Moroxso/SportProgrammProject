using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SportProgramm.Scripts
{
    public class SqlServerDatabaseService : IDatabaseService
    {
        private SportProgrammProjectEntities _context;
        public bool IsConnected { get; private set; }
        public SportProgrammProjectEntities Context => _context;

        public void Initialize()
        {
            try
            {
                _context = new SportProgrammProjectEntities();

                // Простая проверка подключения
                _context.Database.Connection.Open();
                _context.Database.Connection.Close();

                IsConnected = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Server недоступен: {ex.Message}");
                IsConnected = false;
            }
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
