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
                // Инициализируем базу данных БЕЗ показа сообщений
                DatabaseManager.Initialize();

                if (DatabaseManager.IsConnected())
                {
                    // Успешно подключились - открываем ОДНО главное окно
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // НЕ создаем второе окно здесь!
                }
                else
                {
                    MessageBox.Show("Не удалось подключиться к базе данных. Приложение будет закрыто.");
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при запуске: {ex.Message}");
                Shutdown();
            }
        }
    }
}
