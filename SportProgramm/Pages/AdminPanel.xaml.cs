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

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();

        public AdminPanel()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем данные из всех таблиц
                db.Sports.Load();
                db.Cup.Load();
                db.Sportman.Load();

                // Загружаем виды спорта
                SportsList.ItemsSource = db.Sports.Local.ToList();

                // Загружаем последние 5 турниров
                TournamentsList.ItemsSource = db.Cup.Local
                    .OrderByDescending(t => t.Id)
                    .Take(5)
                    .ToList();

                // Загружаем последних 5 спортсменов
                SportsmenList.ItemsSource = db.Sportman.Local
                    .OrderByDescending(s => s.Id)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Управление видами спорта
        private void AddSport_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditSportWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    var sport = addWindow.GetSport();
                    db.Sports.Add(sport);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Вид спорта успешно добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void DeleteSport_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int sportId = (int)button.Tag;
                var sport = db.Sports.Find(sportId);
                if (sport != null)
                {
                    var result = MessageBox.Show($"Удалить вид спорта '{sport.Name}'?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        db.Sports.Remove(sport);
                        db.SaveChanges();
                        LoadData();
                    }
                }
            }
        }

        // Управление турнирами
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
                    LoadData();
                    MessageBox.Show("Турнир успешно добавлен");
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

                            // Обновляем поля
                            tournament.Name = updatedTournament.Name;
                            tournament.Place = updatedTournament.Place;
                            tournament.Score = updatedTournament.Score;
                            tournament.Date = updatedTournament.Date; // Теперь это строка
                            tournament.IdSport = updatedTournament.IdSport;

                            db.SaveChanges();
                            LoadData();
                            MessageBox.Show("Турнир успешно обновлен");
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
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        db.Cup.Remove(tournament);
                        db.SaveChanges();
                        LoadData();
                    }
                }
            }
        }

        private void AddSportsman_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditSportsmanWindow(db.Sports.ToList());
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    var sportsman = addWindow.GetSportsman();
                    db.Sportman.Add(sportsman);
                    db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Спортсмен успешно добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }


        private void EditSportsman_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int sportsmanId = (int)button.Tag;
                var sportsman = db.Sportman.Find(sportsmanId);
                if (sportsman != null)
                {
                    var editWindow = new AddEditSportsmanWindow(db.Sports.ToList(), sportsman);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                }
            }
        }

        private void DeleteSportsman_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int sportsmanId = (int)button.Tag;
                var sportsman = db.Sportman.Find(sportsmanId);
                if (sportsman != null)
                {
                    var result = MessageBox.Show($"Удалить спортсмена '{sportsman.Name}'?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        db.Sportman.Remove(sportsman);
                        db.SaveChanges();
                        LoadData();
                    }
                }
            }
        }
    }
}
    

