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

                // Пробуем изменить строку подключения если нужно
                var config = DatabaseConfig.Load();
                if (!string.IsNullOrEmpty(config.Server) && !string.IsNullOrEmpty(config.Database))
                {
                    var connectionString = BuildConnectionString(config);
                    _context.Database.Connection.ConnectionString = connectionString;
                }

                TestConnection();
            }
            catch
            {
                IsConnected = false;
            }
        }

        private string BuildConnectionString(DatabaseConfig config)
        {
            if (config.UseWindowsAuth)
            {
                return $@"Data Source={config.Server};Initial Catalog={config.Database};Integrated Security=True;MultipleActiveResultSets=True";
            }
            else
            {
                return $@"Data Source={config.Server};Initial Catalog={config.Database};User Id={config.Username};Password={config.Password};MultipleActiveResultSets=True";
            }
        }

        public void TestConnection()
        {
            try
            {
                IsConnected = _context.Database.Exists();
                if (IsConnected)
                {
                    // Простая проверка - пытаемся выполнить запрос
                    var test = _context.Sports.FirstOrDefault();
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

                string backupCommand = $@"BACKUP DATABASE [{_context.Database.Connection.Database}] 
                                   TO DISK = '{backupFile}'";

                _context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, backupCommand);

                MessageBox.Show($"Бэкап создан: {backupFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания бэкапа: {ex.Message}");
            }
        }

        public void RestoreFromBackup(string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                MessageBox.Show("Файл бэкапа не найден");
                return;
            }

            try
            {
                string databaseName = _context.Database.Connection.Database;
                string masterConnectionString = _context.Database.Connection.ConnectionString
                    .Replace(databaseName, "master");

                using (var masterConnection = new System.Data.SqlClient.SqlConnection(masterConnectionString))
                {
                    masterConnection.Open();

                    string restoreCommand = $@"
                    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE [{databaseName}] FROM DISK = '{backupPath}' WITH REPLACE;
                    ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                    using (var command = new System.Data.SqlClient.SqlCommand(restoreCommand, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                TestConnection();
                MessageBox.Show("База данных успешно восстановлена из бэкапа");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления: {ex.Message}");
            }
        }
    }
}
