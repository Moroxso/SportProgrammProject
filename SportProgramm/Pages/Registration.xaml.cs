
using System;
﻿using SportProgramm.BaseDate;
using SportProgramm.Scripts;
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
using System.Xml.Linq;

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }
private void Password_Changed(object sender, RoutedEventArgs e)
{
    string password = psbPass.Password;
    string confirmPassword = txbPass.Text;
    
    bool passwordsMatch = password == confirmPassword;
    bool isPasswordStrong = IsPasswordStrong(password);
    
    // Обновляем подсказку
    if (PasswordRequirements != null)
    {
        if (password.Length == 0)
        {
            PasswordRequirements.Text = "Пароль должен содержать минимум 8 символов, буквы и цифры";
            PasswordRequirements.Foreground = Brushes.Gray;
        }
        else if (!isPasswordStrong)
        {
            PasswordRequirements.Text = "Слабый пароль - добавьте цифры и буквы";
            PasswordRequirements.Foreground = Brushes.Orange;
        }
        else
        {
            PasswordRequirements.Text = "Надежный пароль";
            PasswordRequirements.Foreground = Brushes.Green;
        }
    }
    
    if (!passwordsMatch || !isPasswordStrong)
    {
        ButtonRegistration.IsEnabled = false;
        
        if (!passwordsMatch)
        {
            psbPass.Background = Brushes.LightCoral;
            psbPass.BorderBrush = Brushes.Red;
        }
        else
        {
            psbPass.Background = Brushes.LightYellow;
            psbPass.BorderBrush = Brushes.Orange;
        }
    }
    else
    {
        ButtonRegistration.IsEnabled = true;
        psbPass.Background = Brushes.LightGreen;
        psbPass.BorderBrush = Brushes.Green;
    }
}

        // Метод проверки сложности пароля
        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasLetter = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c))
                    hasLetter = true;
                else if (char.IsDigit(c))
                    hasDigit = true;

                // Если уже нашли и букву и цифру, можно выйти из цикла
                if (hasLetter && hasDigit)
                    break;
            }

            return hasLetter && hasDigit;
        }

        private void ButtonReg(object sender, RoutedEventArgs e)
        {
            // Проверяем сложность пароля перед регистрацией
            if (!IsPasswordStrong(psbPass.Password))
            {
                MessageBox.Show("Пароль должен содержать минимум 8 символов, включая буквы и цифры!",
                               "Слабый пароль",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                return;
            }

            if (psbPass.Password != txbPass.Text)
            {
                MessageBox.Show("Пароли не совпадают!",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
                return;
            }

            if (AppConnect.model0db.Users.Count(x => x.Login == txbLogin.Text) > 0)
            {
                MessageBox.Show("Пользователь с таким логином уже существует!",
                               "Уведомление",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
                return;
            }

            try
            {
                Users userObj = new Users()
                {
                    Name = txbLogin.Text, // Добавь поле для имени если нужно
                    Login = txbLogin.Text,
                    Password = psbPass.Password,
                    IdRole = 2 // Обычный пользователь
                };

                AppConnect.model0db.Users.Add(userObj);
                AppConnect.model0db.SaveChanges();

                // Автоматически входим после регистрации
                CurrentUser.User = userObj;

                // Обновляем интерфейс главного окна
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.RefreshUserInterface();

                MessageBox.Show("Регистрация прошла успешно! Добро пожаловать!",
                               "Уведомление",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

                // Переходим на главную страницу
                AppFrame.frameMain.Navigate(new Home());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private void ButtonEsc(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
