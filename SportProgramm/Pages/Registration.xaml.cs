
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
            if (psbPass.Password != txbPass.Text)
            {
                ButtonRegistration.IsEnabled = false;
                psbPass.Background = Brushes.LightCoral;
                psbPass.BorderBrush = Brushes.LightCoral;
            }
            else
            {
                ButtonRegistration.IsEnabled = true;
                psbPass.Background = Brushes.LightGreen;
                psbPass.BorderBrush = Brushes.LightGreen;
            }
        }

        private void ButtonReg(object sender, RoutedEventArgs e)
        {
            if (AppConnect.model0db.Users.Count(x => x.Login == txbLogin.Text) > 0)
            {
                MessageBox.Show("Пользователь с такими логином есть!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                Users userObj = new Users()
                {
                    Name = txbLogin.Text,
                    Login = txbLogin.Text,
                    Password = txbPass.Text,
                    IdRole = 2
                };
                AppConnect.model0db.Users.Add(userObj);


                AppConnect.model0db.SaveChanges();
                MessageBox.Show("Данные успешно добавлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при добавление данных!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEsc(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }
    }
}
