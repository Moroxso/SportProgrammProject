using SportProgramm.BaseDate;
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
using System.Windows.Shapes;
using System.Data.Entity;
using System.Collections.ObjectModel;
using SportProgramm.Scripts;

namespace SportProgramm.Pages.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для ManageTournamentParticipantsWindow.xaml
    /// </summary>
    public partial class ManageTournamentParticipantsWindow : Window
    {
        private SportProgrammProjectEntities db = DatabaseManager.GetContext();
        private Cup _tournament;

        // Коллекции для DataGrid
        private ObservableCollection<SportsmanItem> _availableSportsmen = new ObservableCollection<SportsmanItem>();
        private ObservableCollection<ParticipantItem> _currentParticipants = new ObservableCollection<ParticipantItem>();

        public ManageTournamentParticipantsWindow(Cup tournament)
        {
            InitializeComponent();
            _tournament = tournament;

            // Прямое присваивание ItemsSource в XAML не работает, делаем в коде
            AvailableSportsmenGrid.ItemsSource = _availableSportsmen;
            ParticipantsGrid.ItemsSource = _currentParticipants;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем данные из базы
                db.Sportman.Load();
                db.Sports.Load();
                db.TournamentResults.Load();

                // Устанавливаем информацию о турнире
                TournamentTitle.Text = _tournament.Name;
                TournamentInfo.Text = $"{_tournament.Place} • {_tournament.Date:dd.MM.yyyy}";

                // Загружаем виды спорта для фильтра
                var sports = db.Sports.ToList();
                SportFilterComboBox.Items.Clear();
                SportFilterComboBox.Items.Add(new Sports { Id = 0, Name = "Все виды спорта" });
                foreach (var sport in sports)
                {
                    SportFilterComboBox.Items.Add(sport);
                }
                SportFilterComboBox.SelectedIndex = 0;

                // Загружаем участников и доступных спортсменов
                LoadParticipants();
                UpdateAvailableSportsmen();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadParticipants()
        {
            _currentParticipants.Clear();

            var participants = db.TournamentResults
                .Where(tr => tr.IdCup == _tournament.Id)
                .ToList();

            foreach (var result in participants)
            {
                var player = db.Sportman.Find(result.IdPlayer);
                if (player != null)
                {
                    _currentParticipants.Add(new ParticipantItem
                    {
                        ResultId = result.Id,
                        PlayerId = player.Id,
                        PlayerName = player.Name,
                        Team = player.Team,
                        Position = result.Position?.ToString() ?? "",
                        Score = result.Score ?? "",
                        Points = result.Points?.ToString() ?? "0"
                    });
                }
            }
        }

        private void UpdateAvailableSportsmen()
        {
            _availableSportsmen.Clear();

            // Получаем ID уже добавленных участников
            var currentPlayerIds = _currentParticipants.Select(p => p.PlayerId).ToList();

            // Все спортсмены, которые еще не добавлены
            var availablePlayers = db.Sportman.Local
                .Where(s => !currentPlayerIds.Contains(s.Id))
                .ToList();

            foreach (var player in availablePlayers)
            {
                _availableSportsmen.Add(new SportsmanItem
                {
                    Id = player.Id,
                    Name = player.Name,
                    Team = player.Team,
                    Lvl = player.Lvl,
                    SportsText = GetPlayerSportsText(player),
                    IsSelected = false
                });
            }
        }

        private string GetPlayerSportsText(Sportman player)
        {
            var sports = new List<string>();

            if (player.IdSport_1 > 0)
            {
                var sport = db.Sports.Find(player.IdSport_1);
                if (sport != null) sports.Add(sport.Name);
            }

            return string.Join(", ", sports);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SportFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var searchText = SearchTextBox.Text?.ToLower() ?? "";
            var selectedSport = SportFilterComboBox.SelectedItem as Sports;

            // Получаем ID уже добавленных участников
            var currentPlayerIds = _currentParticipants.Select(p => p.PlayerId).ToList();

            // Фильтруем всех спортсменов из базы
            var filteredPlayers = db.Sportman.Local
                .Where(s => !currentPlayerIds.Contains(s.Id) &&
                           (string.IsNullOrEmpty(searchText) || s.Name.ToLower().Contains(searchText)) &&
                           (selectedSport == null || selectedSport.Id == 0 || s.IdSport_1 == selectedSport.Id))
                .ToList();

            // Обновляем коллекцию
            _availableSportsmen.Clear();
            foreach (var player in filteredPlayers)
            {
                _availableSportsmen.Add(new SportsmanItem
                {
                    Id = player.Id,
                    Name = player.Name,
                    Team = player.Team,
                    Lvl = player.Lvl,
                    SportsText = GetPlayerSportsText(player),
                    IsSelected = false
                });
            }
        }

        private void AddSelectedParticipants_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = _availableSportsmen.Where(s => s.IsSelected).ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите спортсменов для добавления");
                return;
            }

            foreach (var item in selectedItems)
            {
                // Добавляем в базу
                var newResult = new TournamentResults
                {
                    IdCup = _tournament.Id,
                    IdPlayer = item.Id,
                    Position = null,
                    Score = "",
                    Points = null
                };
                db.TournamentResults.Add(newResult);

                // Добавляем в список участников
                _currentParticipants.Add(new ParticipantItem
                {
                    ResultId = newResult.Id,
                    PlayerId = item.Id,
                    PlayerName = item.Name,
                    Team = item.Team,
                    Position = "",
                    Score = "",
                    Points = "0"
                });

                // Убираем выделение
                item.IsSelected = false;
            }

            // Сохраняем и обновляем
            db.SaveChanges();
            UpdateAvailableSportsmen(); // Обновляем список доступных

            MessageBox.Show($"Добавлено {selectedItems.Count} участников");
        }

        private void RemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int resultId = (int)button.Tag;

                // Находим в базе
                var result = db.TournamentResults.Find(resultId);
                if (result != null)
                {
                    // Удаляем из базы
                    db.TournamentResults.Remove(result);

                    // Удаляем из списка
                    var itemToRemove = _currentParticipants.FirstOrDefault(p => p.ResultId == resultId);
                    if (itemToRemove != null)
                    {
                        _currentParticipants.Remove(itemToRemove);
                    }

                    db.SaveChanges();
                    UpdateAvailableSportsmen(); // Обновляем список доступных
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сохраняем изменения в результатах
                foreach (var participant in _currentParticipants)
                {
                    var result = db.TournamentResults.Find(participant.ResultId);
                    if (result != null)
                    {
                        result.Position = int.TryParse(participant.Position, out int pos) ? pos : (int?)null;
                        result.Score = participant.Score;
                        result.Points = int.TryParse(participant.Points, out int points) ? points : (int?)null;
                    }
                }

                db.SaveChanges();
                MessageBox.Show("Изменения сохранены успешно!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    // Классы для данных
    public class SportsmanItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Lvl { get; set; }
        public string SportsText { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ParticipantItem
    {
        public int ResultId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public string Score { get; set; }
        public string Points { get; set; }
    }
}



