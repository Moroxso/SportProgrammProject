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

        private void ButtonLogin(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Login());
        }

        private void ButtonRegistrator(object sender, RoutedEventArgs e)
        {
            FrmMain.Navigate(new Registration());
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
    }
}
