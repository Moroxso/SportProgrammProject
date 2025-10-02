using SportProgramm.BaseDate;
using SportProgramm.Scripts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SportProgramm.Pages.AdminPanelPages;

namespace SportProgramm
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Просто инициализируем базу данных
                DatabaseManager.Initialize();

                // Если добрались сюда - подключение успешно
                // Главное окно создастся автоматически через StartupUri
            }
            catch
            {
                // Если произошла ошибка - приложение закроется
                Shutdown();
            }
        }
    }
}

