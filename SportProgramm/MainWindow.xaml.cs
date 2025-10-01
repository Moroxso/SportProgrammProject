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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SportProgramm.Pages;
using SportProgramm.BaseDate;


namespace SportProgramm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppConnect.model0db = new SportProgrammProjectEntities();
            AppFrame.frameMain = FrmMain;

            FrmMain.Navigate(new Home());
        }

        private void ButtonHome(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Home());
        }

        private void ButtonTournaments(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Tournaments());
        }

        private void ButtonTop(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new RatingOfSportsMans());
        }

        private void UpdateUserInterface()
        {
            if (CurrentUser.IsAuthenticated)
            {
                // Пользователь авторизован
                UserInfoText.Text = CurrentUser.DisplayName;
                UserInfoText.Visibility = Visibility.Visible;

                LoginButton.Visibility = Visibility.Collapsed;
                RegisterButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;

                // Показываем админ-панель только админам
                AdminButton.Visibility = CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                // Пользователь не авторизован
                UserInfoText.Visibility = Visibility.Collapsed;
                AdminButton.Visibility = Visibility.Collapsed;

                LoginButton.Visibility = Visibility.Visible;
                RegisterButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;
            }
        }

        // Обработчик входа (перенаправляем на страницу входа)
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Login());
        }

        // Обработчик регистрации
        private void ButtonRegistrator_Click(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Registration());
        }

        // Обработчик выхода
        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.User = null;
            UpdateUserInterface();
            FrmMain.Navigate(new Home());

            MessageBox.Show("Вы успешно вышли из системы", "Выход",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Обработчик админ-панели
        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.IsAdmin)
            {
                FrmMain.Navigate(new AdminPanel());
            }
        }

        // Метод для обновления интерфейса из других страниц
        public void RefreshUserInterface()
        {
            UpdateUserInterface();
        }

    }
}
