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
            try
            {
                // Просто используем LocalDatabaseService - он сам разберется
                var localService = new LocalDatabaseService();
                localService.Initialize();

                if (localService.IsConnected)
                {
                    _currentDatabaseService = localService;
                    CurrentDatabaseType = DatabaseType.LocalDB;
                    return;
                }

                throw new Exception("Не удалось подключиться к базе данных");
            }
            catch (Exception ex)
            {
                // Показываем сообщение об ошибке
                MessageBox.Show(ex.Message, "Ошибка базы данных",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public static SportProgrammProjectEntities GetContext()
        {
            return _currentDatabaseService?.Context;
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
