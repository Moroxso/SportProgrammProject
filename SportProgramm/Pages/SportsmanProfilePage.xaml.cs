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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для SportsmanProfilePage.xaml
    /// </summary>
    public partial class SportsmanProfilePage : Page
    {
        private SportProgrammProjectEntities db = DatabaseManager.GetContext();
        private int _playerId;

        public SportsmanProfilePage(int playerId)
        {
            InitializeComponent();
            _playerId = playerId;
            LoadSportsmanProfile();
        }

        private void LoadSportsmanProfile()
        {
            try
            {
                // Загружаем спортсмена
                var sportsman = db.Sportman.FirstOrDefault(s => s.Id == _playerId);

                if (sportsman != null)
                {
                    // Загружаем результаты турниров
                    var tournamentResults = db.TournamentResults
                        .Where(tr => tr.IdPlayer == _playerId)
                        .ToList();

                    // Создаем ViewModel
                    var viewModel = new SportsmanProfileViewModel
                    {
                        PlayerName = sportsman.Name,
                        Team = sportsman.Team,
                        Level = sportsman.Lvl,
                        BestResult = GetBestResult(tournamentResults)
                    };

                    // Устанавливаем DataContext
                    DataContext = viewModel;

                    // Загружаем список турниров
                    LoadTournamentsList(tournamentResults);
                }
                else
                {
                    MessageBox.Show("Спортсмен не найден");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void LoadTournamentsList(List<TournamentResults> results)
        {
            var tournamentsList = new List<SportsmanTournamentViewModel>();

            foreach (var result in results.OrderByDescending(tr => tr.Cup.Date))
            {
                var tournament = db.Cup.FirstOrDefault(c => c.Id == result.IdCup);
                if (tournament != null)
                {
                    tournamentsList.Add(new SportsmanTournamentViewModel
                    {
                        TournamentId = tournament.Id,
                        TournamentName = tournament.Name,
                        TournamentPlace = tournament.Place,
                        TournamentDate = tournament.Date,
                        Position = result.Position.HasValue ? result.Position.Value.ToString() : "-",
                        Score = result.Score ?? ""
                    });
                }
            }

            TournamentsList.ItemsSource = tournamentsList;
        }

        private string GetBestResult(List<TournamentResults> results)
        {
            if (results == null || !results.Any())
                return "Нет данных";

            TournamentResults bestResult = null;

            foreach (var result in results)
            {
                if (result.Position.HasValue)
                {
                    if (bestResult == null || result.Position.Value < bestResult.Position.Value)
                    {
                        bestResult = result;
                    }
                }
            }

            if (bestResult != null)
            {
                var tournament = db.Cup.FirstOrDefault(c => c.Id == bestResult.IdCup);
                if (tournament != null)
                {
                    return $"{bestResult.Position.Value} место - {tournament.Name}";
                }
            }

            return "Нет данных";
        }

        private void TournamentItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is SportsmanTournamentViewModel tournament)
            {
                NavigationService.Navigate(new TournamentDetailsPage(tournament.TournamentId));
            }
        }
    }

    // ViewModel для профиля спортсмена
    public class SportsmanProfileViewModel
    {
        public string PlayerName { get; set; }
        public string Team { get; set; }
        public string Level { get; set; }
        public string BestResult { get; set; }
    }

    public class SportsmanTournamentViewModel
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public string TournamentPlace { get; set; }
        public DateTime TournamentDate { get; set; }
        public string Position { get; set; }
        public string Score { get; set; }
    }
}



