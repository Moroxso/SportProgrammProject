using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportProgramm.Scripts
{
    public static class DatabaseHelper
    {
        public static SportProgrammProjectEntities GetContext()
        {
            // Если используешь DatabaseManager, то через него
            // Или просто создаем новый контекст
            return new SportProgrammProjectEntities();
        }

        private static IDatabaseService GetDatabaseService()
        {
            // Возвращает твой сервис базы данных
            return new LocalDatabaseService(); // или SqlServerDatabaseService
        }

    }
}
