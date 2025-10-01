using SportProgramm;
using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SportProgramm.Scripts
{
    public static class DatabaseManager
    {
        private static IDatabaseService _currentDatabaseService;
        public static DatabaseType CurrentDatabaseType { get; private set; }

        public enum DatabaseType
        {
            SqlServer,
            LocalDB
        }

        public static void Initialize()
        {
            // Пробуем разные типы баз данных в порядке приоритета
            var databaseServices = new (IDatabaseService service, DatabaseType type)[]
            {
            (new SqlServerDatabaseService(), DatabaseType.SqlServer),
            (new LocalDatabaseService(), DatabaseType.LocalDB)
            };

            foreach (var (service, type) in databaseServices)
            {
                try
                {
                    service.Initialize();
                    if (service.IsConnected)
                    {
                        _currentDatabaseService = service;
                        CurrentDatabaseType = type;

                        // ТИХОЕ подключение - без MessageBox
                        System.Diagnostics.Debug.WriteLine($"Успешное подключение: {type}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка {type}: {ex.Message}");
                }
            }

            throw new Exception("Не удалось подключиться ни к одной базе данных");
        }

        public static SportProgrammProjectEntities GetContext()
        {
            if (_currentDatabaseService == null)
            {
                throw new InvalidOperationException("DatabaseManager не инициализирован.");
            }
            return _currentDatabaseService.Context;
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
    }
}
