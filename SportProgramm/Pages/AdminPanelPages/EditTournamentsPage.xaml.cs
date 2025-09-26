using SportProgramm.BaseDate;
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

namespace SportProgramm.Pages.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для EditTournamentsPage.xaml
    /// </summary>
    public partial class EditTournamentsPage : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();

        public EditTournamentsPage()
        {
            InitializeComponent();
            LoadTournaments();
        }

        private void LoadTournaments()
        {
            try
            {
                db.Cup.Load();
                db.Sports.Load();

                // Загружаем турниры с включением данных о виде спорта
                TournamentsList.ItemsSource = db.Cup.Local.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void AddTournament_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditTournamentWindow(db.Sports.ToList());
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    var tournament = addWindow.GetTournament();
                    db.Cup.Add(tournament);
                    db.SaveChanges();
                    LoadTournaments();
                    MessageBox.Show("Турнир добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void EditTournament_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int tournamentId = (int)button.Tag;
                var tournament = db.Cup.Find(tournamentId);
                if (tournament != null)
                {
                    var editWindow = new AddEditTournamentWindow(db.Sports.ToList(), tournament);
                    if (editWindow.ShowDialog() == true)
                    {
                        try
                        {
                            var updatedTournament = editWindow.GetTournament();
                            tournament.Name = updatedTournament.Name;
                            tournament.Place = updatedTournament.Place;
                            tournament.Score = updatedTournament.Score;
                            tournament.Date = updatedTournament.Date;
                            tournament.IdSport = updatedTournament.IdSport;
                            db.SaveChanges();
                            LoadTournaments();
                            MessageBox.Show("Турнир обновлен");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void DeleteTournament_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int tournamentId = (int)button.Tag;
                var tournament = db.Cup.Find(tournamentId);
                if (tournament != null)
                {
                    var result = MessageBox.Show($"Удалить турнир '{tournament.Name}'?",
                        "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.Cup.Remove(tournament);
                            db.SaveChanges();
                            LoadTournaments();
                            MessageBox.Show("Турнир удален");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
