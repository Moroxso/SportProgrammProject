using SportProgramm.BaseDate;
using SportProgramm.Scripts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tournaments.xaml
    /// </summary>
    public partial class Tournaments : Page
    {
        private SportProgrammProjectEntities db = DatabaseManager.GetContext();
        public Tournaments()
        {
            InitializeComponent();
            LoadTournaments();
        }

        private void LoadTournaments()
        {
            try
            {
                db.Cup.Load();
                TournamentsList.ItemsSource = db.Cup.Local.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void TournamentItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Cup tournament)
            {
                // Переход на страницу деталей турнира
                NavigationService.Navigate(new TournamentDetailsPage(tournament.Id));
            }
        }
    }
}
