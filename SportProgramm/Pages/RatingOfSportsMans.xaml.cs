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
using SportProgramm.BaseDate;

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для RatingOfSportsMans.xaml
    /// </summary>
    public partial class RatingOfSportsMans : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();

        public RatingOfSportsMans()
        {
            InitializeComponent();
            LoadTopSportsmen();
        }


        private void LoadTopSportsmen()
        {
            try
            {
                // Загружаем данные
                db.Sportman.Load();
                db.Sports.Load();

                // Создаем список для отображения
                var sportsmenData = new List<SportsmanViewModel>();

                // Обрабатываем каждого спортсмена
                int rank = 1;
                foreach (var sportsman in db.Sportman.Local.OrderByDescending(s => GetSportsmanScore(s)))
                {
                    var viewModel = new SportsmanViewModel
                    {
                        Rank = rank++,
                        Id = sportsman.Id,
                        Name = sportsman.Name,
                        Date = sportsman.Date,
                        Team = sportsman.Team,
                        Lvl = sportsman.Lvl,
                        Sports = GetSportsmanSports(sportsman)
                    };

                    sportsmenData.Add(viewModel);
                }

                SportsmenList.ItemsSource = sportsmenData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Метод для получения видов спорта спортсмена
        private List<string> GetSportsmanSports(Sportman sportsman)
        {
            var sports = new List<string>();

            // Проверяем каждый вид спорта (используем проверку на 0 вместо HasValue)
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

        // Метод для расчета "рейтинга" спортсмена
        private int GetSportsmanScore(Sportman sportsman)
        {
            int score = 0;

            // Простая логика: больше видов спорта = выше рейтинг
            if (sportsman.IdSport_1 > 0) score += 100;
            if (sportsman.IdSport_2 > 0) score += 100;
            if (sportsman.IdSport_3 > 0) score += 100;
            if (sportsman.IdSport_4 > 0) score += 100;
            if (sportsman.IdSport_5 > 0) score += 100;

            return score;
        }
    }

    // ViewModel для отображения данных спортсмена
    public class SportsmanViewModel
    {
        public int Rank { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Team { get; set; }
        public string Lvl { get; set; }
        public List<string> Sports { get; set; }
    }

}

