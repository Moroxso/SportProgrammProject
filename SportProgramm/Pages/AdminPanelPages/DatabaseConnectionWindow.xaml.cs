using SportProgramm.BaseDate;
using SportProgramm.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SportProgramm.Pages.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для DatabaseConnectionWindow.xaml
    /// </summary>
    public partial class DatabaseConnectionWindow : Window
    {
        public DatabaseConnectionWindow()
        {
            InitializeComponent();

            // Инициализируем после загрузки окна
            Loaded += DatabaseConnectionWindow_Loaded;
        }

        private void DatabaseConnectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Теперь элементы гарантированно инициализированы
            UpdateStatus();
        }

        // Обработчик изменения типа подключения
        private void ConnectionType_Changed(object sender, RoutedEventArgs e)
        {
            if (SqlServerPanel != null && SqlServerRadio != null)
            {
                SqlServerPanel.Visibility = SqlServerRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                UpdateStatus();
            }
        }

        // Обработчик изменения типа аутентификации
        private void AuthType_Changed(object sender, RoutedEventArgs e)
        {
            if (SqlAuthPanel != null && WindowsAuthCheckBox != null)
            {
                SqlAuthPanel.Visibility = WindowsAuthCheckBox.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        // Обновление статуса подключения
        private void UpdateStatus()
        {
            if (StatusText != null && LocalDbRadio != null)
            {
                if (LocalDbRadio.IsChecked == true)
                {
                    StatusText.Text = "Будет использована локальная база данных. Данные сохраняются на этом компьютере.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    StatusText.Text = "Подключение к SQL Server. Требуется доступ к серверу базы данных.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Blue;
                }
            }
        }

        // Обработчик кнопки "Подключиться"
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Если выбран LocalDB, просто продолжаем
                if (LocalDbRadio?.IsChecked == true)
                {
                    DialogResult = true;
                    Close();
                }
                else if (SqlServerRadio?.IsChecked == true)
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        // Обработчик кнопки "Отмена"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
    

