using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportProgramm;

namespace SportProgramm.Scripts
{
    public static class DatabaseManager
    {
        private static IDatabaseService _currentDatabaseService;
        public static DatabaseType CurrentDatabaseType { get; private set; }
        public static event Action<string> ConnectionStatusChanged;

        public enum DatabaseType
        {
            SqlServer,
            LocalDB
        }

        public static void Initialize()
        {
            // Сначала пробуем подключиться к SQL Server
            var sqlService = new SqlServerDatabaseService();
            sqlService.Initialize();

            if (sqlService.IsConnected)
            {
                _currentDatabaseService = sqlService;
                CurrentDatabaseType = DatabaseType.SqlServer;
                ConnectionStatusChanged?.Invoke("Подключено к SQL Server");
            }
            else
            {
                // Если SQL Server недоступен, используем LocalDB
                var localService = new LocalDatabaseService();
                localService.Initialize();

                if (localService.IsConnected)
                {
                    _currentDatabaseService = localService;
                    CurrentDatabaseType = DatabaseType.LocalDB;
                    ConnectionStatusChanged?.Invoke("Используется локальная база данных");
                }
                else
                {
                    throw new Exception("Не удалось подключиться ни к одной базе данных");
                }
            }
        }

        public static SportProgrammProjectEntities GetContext()
        {
            return _currentDatabaseService?.GetContext();
        }

        public static bool IsConnected()
        {
            return _currentDatabaseService?.IsConnected ?? false;
        }

        public static void CreateBackup()
        {
            _currentDatabaseService?.CreateBackup();
        }

        public static void RestoreFromBackup(string backupPath)
        {
            _currentDatabaseService?.RestoreFromBackup(backupPath);
        }

        public static void SwitchToLocalDB()
        {
            var localService = new LocalDatabaseService();
            localService.Initialize();

            if (localService.IsConnected)
            {
                _currentDatabaseService = localService;
                CurrentDatabaseType = DatabaseType.LocalDB;
                ConnectionStatusChanged?.Invoke("Переключено на локальную базу данных");
            }
        }
    }
}
