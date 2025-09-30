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
                // Инициализируем базу данных
                DatabaseManager.Initialize();

                if (!DatabaseManager.IsConnected())
                {
                    MessageBox.Show("Не удалось подключиться к базе данных. Приложение будет закрыто.");
                    Shutdown();
                    return;
                }

                // Показываем окно подключения
                var connectionWindow = new DatabaseConnectionWindow();
                if (connectionWindow.ShowDialog() == true)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка: {ex.Message}");
                Shutdown();
            }
        }
    }
}
