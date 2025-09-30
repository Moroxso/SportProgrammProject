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
    public class SqlServerDatabaseService : IDatabaseService
    {
        private SportProgrammProjectEntities _context;
        public bool IsConnected { get; private set; }

        public void Initialize()
        {
            try
            {
                _context = new SportProgrammProjectEntities();
                TestConnection();
            }
            catch
            {
                IsConnected = false;
            }
        }

        public void TestConnection()
        {
            try
            {
                IsConnected = _context.Database.Exists();
                if (IsConnected)
                {
                    // Проверяем, что все таблицы существуют
                    var connection = _context.Database.Connection;
                    connection.Open();
                    connection.Close();
                }
            }
            catch
            {
                IsConnected = false;
            }
        }

        public void CreateBackup()
        {
            if (!IsConnected) return;

            try
            {
                string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                Directory.CreateDirectory(backupDir);

                string backupFile = Path.Combine(backupDir, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

                // SQL команда для бэкапа
                string backupCommand = $@"BACKUP DATABASE [{_context.Database.Connection.Database}] 
                                   TO DISK = '{backupFile}'";

                _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, backupCommand);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания бэкапа: {ex.Message}");
            }
        }

        public void RestoreFromBackup(string backupPath)
        {
            if (!File.Exists(backupPath)) return;

            try
            {
                // Переключаемся на master базу для восстановления
                var masterConnectionString = _context.Database.Connection.ConnectionString
                    .Replace(_context.Database.Connection.Database, "master");

                using (var masterContext = new SportProgrammProjectEntities(masterConnectionString))
                {
                    string restoreCommand = $@"USE master;
                                        ALTER DATABASE [{_context.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                        RESTORE DATABASE [{_context.Database.Connection.Database}] 
                                        FROM DISK = '{backupPath}' WITH REPLACE;
                                        ALTER DATABASE [{_context.Database.Connection.Database}] SET MULTI_USER;";

                    masterContext.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, restoreCommand);
                }

                TestConnection();
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
