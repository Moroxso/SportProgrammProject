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
using SportProgramm.BaseDate;
using System.Data.Entity;


namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();
        public Home()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем данные
                db.Cup.Load();
                db.Sportman.Load();
                db.Sports.Load();

                // Загружаем недавние турниры (последние 10 по ID)
                var recentTournaments = db.Cup.Local
                    .OrderByDescending(t => t.Id)
                    .Take(10)
                    .Select(t => new TournamentViewModel
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Place = t.Place,
                        Date = t.Date,
                        Score = t.Score
                    })
                    .ToList();

                RecentTournamentsList.ItemsSource = recentTournaments;

                // Загружаем топ 5 спортсменов
                var topSportsmen = db.Sportman.Local
                    .OrderByDescending(s => GetSportsmanScore(s))
                    .Take(5)
                    .Select((s, index) => new SportsmanViewModel
                    {
                        Rank = index + 1,
                        Id = s.Id,
                        Name = s.Name,
                        Team = s.Team,
                        Lvl = s.Lvl,
                        Sports = GetSportsmanSports(s)
                    })
                    .ToList();

                TopSportsmenList.ItemsSource = topSportsmen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Методы из предыдущего кода (адаптированные)
        private List<string> GetSportsmanSports(Sportman sportsman)
        {
            var sports = new List<string>();

            if (sportsman.IdSport_1 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_1);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_2 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_2);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_3 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_3);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_4 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_4);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_5 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_5);
                if (sport != null) sports.Add(sport.Name);
            }

            return sports;
        }

        private int GetSportsmanScore(Sportman sportsman)
        {
            int score = 0;

            if (sportsman.IdSport_1 > 0) score += 100;
            if (sportsman.IdSport_2 > 0) score += 100;
            if (sportsman.IdSport_3 > 0) score += 100;
            if (sportsman.IdSport_4 > 0) score += 100;
            if (sportsman.IdSport_5 > 0) score += 100;

            return score;
        }
    }

    // ViewModel для турниров
    public class TournamentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public DateTime? Date { get; set; }
        public string Score { get; set; }
    }


}

