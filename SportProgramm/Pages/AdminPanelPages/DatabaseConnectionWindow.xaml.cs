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
            UpdateStatus();
        }

        private void ConnectionType_Changed(object sender, RoutedEventArgs e)
        {
            SqlServerPanel.Visibility = SqlServerRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            UpdateStatus();
        }

        private void AuthType_Changed(object sender, RoutedEventArgs e)
        {
            SqlAuthPanel.Visibility = WindowsAuthCheckBox.IsChecked == true ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateStatus()
        {
            if (LocalDbRadio.IsChecked == true)
            {
                StatusText.Text = "Будет использована локальная база данных. Данные сохраняются на этом компьютере.";
                StatusText.Foreground = Brushes.Green;
            }
            else
            {
                StatusText.Text = "Подключение к SQL Server. Требуется доступ к серверу базы данных.";
                StatusText.Foreground = Brushes.Blue;
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LocalDbRadio.IsChecked == true)
                {
                    DatabaseManager.SwitchToLocalDB();
                }
                // Для SQL Server можно добавить кастомную логику подключения

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
