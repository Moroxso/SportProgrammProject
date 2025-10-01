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
using SportProgramm.Scripts;

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void ButtonLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                var userObj = AppConnect.model0db.Users.FirstOrDefault(x => x.Login == txbLogin.Text && x.Password == psbPassword.Password);
                if (userObj == null)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем пользователя в сессии
                CurrentUser.User = userObj;

                // Получаем главное окно для обновления интерфейса
                var mainWindow = Application.Current.MainWindow as MainWindow;

                string welcomeMessage = CurrentUser.IsAdmin
                    ? $"Здравствуйте, администратор {userObj.Name}!"
                    : $"Здравствуйте, пользователь {userObj.Name}!";

                MessageBox.Show(welcomeMessage, "Успешный вход",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем интерфейс главного окна
                mainWindow?.RefreshUserInterface();

                // Переходим на соответствующую страницу
                if (CurrentUser.IsAdmin)
                {
                    AppFrame.frameMain.Navigate(new AdminPanel());
                }
                else
                {
                    AppFrame.frameMain.Navigate(new Home());
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show($"Ошибка авторизации: {Ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonRegistrator(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new Registration());
        }


    }
}
