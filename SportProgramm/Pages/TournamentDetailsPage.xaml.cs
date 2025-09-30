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
    /// Логика взаимодействия для TournamentDetailsPage.xaml
    /// </summary>
    public partial class TournamentDetailsPage : Page
    {
        private SportProgrammProjectEntities db = DatabaseManager.GetContext();
        private int _tournamentId;

        public TournamentDetailsPage(int tournamentId)
        {
            InitializeComponent();
            _tournamentId = tournamentId;
            LoadTournamentDetails();
        }

        private void LoadTournamentDetails()
        {
            try
            {
                // Загружаем информацию о турнире
                var tournament = db.Cup.FirstOrDefault(c => c.Id == _tournamentId);

                if (tournament == null)
                {
                    MessageBox.Show("Турнир не найден");
                    return;
                }

                // Загружаем вид спорта
                var sport = db.Sports.FirstOrDefault(s => s.Id == tournament.IdSport);

                // Устанавливаем данные контекста
                DataContext = new
                {
                    TournamentName = tournament.Name,
                    TournamentPlace = tournament.Place,
                    TournamentDate = tournament.Date,
                    SportName = sport?.Name ?? "Не указан"
                };

                // Загружаем результаты
                var results = db.TournamentResults
                                .Where(tr => tr.IdCup == _tournamentId)
                                .ToList();

                // Создаем список для отображения
                var displayResults = new List<TournamentResultViewModel>();

                foreach (var result in results)
                {
                    var player = db.Sportman.FirstOrDefault(p => p.Id == result.IdPlayer);

                    displayResults.Add(new TournamentResultViewModel
                    {
                        Position = GetPositionString(result.Position),
                        PlayerName = GetPlayerName(player),
                        Team = GetTeamName(player),
                        Score = result.Score ?? "",
                        Points = GetPointsString(result.Points)
                    });
                }

                // Сортируем по позиции
                ResultsList.ItemsSource = displayResults.OrderBy(r => GetPositionValue(r.Position)).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Вспомогательные методы
        private string GetPositionString(int? position)
        {
            return position.HasValue ? position.Value.ToString() : "-";
        }

        private string GetPlayerName(Sportman player)
        {
            return player?.Name ?? "Неизвестный участник";
        }

        private string GetTeamName(Sportman player)
        {
            return player?.Team ?? "";
        }

        private string GetPointsString(int? points)
        {
            return points.HasValue ? points.Value.ToString() : "0";
        }

        private int GetPositionValue(string position)
        {
            if (int.TryParse(position, out int pos))
                return pos;
            return int.MaxValue;
        }
    }

    public class TournamentResultViewModel
    {
        public string Position { get; set; }
        public string PlayerName { get; set; }
        public string Team { get; set; }
        public string Score { get; set; }
        public string Points { get; set; }
    }
}

