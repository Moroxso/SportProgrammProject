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
    /// Логика взаимодействия для EditSportsmenPage.xaml
    /// </summary>
    public partial class EditSportsmenPage : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();

        public EditSportsmenPage()
        {
            InitializeComponent();
            LoadSportsmen();
        }

        private void LoadSportsmen()
        {
            try
            {
                db.Sportman.Load();
                db.Sports.Load();

                // Создаем список спортсменов с дополнительной информацией о видах спорта
                var sportsmenData = db.Sportman.Local.Select(s => new SportsmanViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Team = s.Team,
                    Lvl = s.Lvl,
                    Date = s.Date,
                    SportsList = GetSportsmanSportsNames(s)
                }).ToList();

                SportsmenList.ItemsSource = sportsmenData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private List<string> GetSportsmanSportsNames(Sportman sportsman)
        {
            var sports = new List<string>();

            if (sportsman.IdSport_1 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_1);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_2.HasValue && sportsman.IdSport_2 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_2.Value);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_3.HasValue && sportsman.IdSport_3 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_3.Value);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_4.HasValue && sportsman.IdSport_4 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_4.Value);
                if (sport != null) sports.Add(sport.Name);
            }

            if (sportsman.IdSport_5.HasValue && sportsman.IdSport_5 > 0)
            {
                var sport = db.Sports.Local.FirstOrDefault(s => s.Id == sportsman.IdSport_5.Value);
                if (sport != null) sports.Add(sport.Name);
            }

            return sports;
        }

        private void AddSportsman_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditSportsmanWindow(db.Sports.ToList());
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    var sportsman = addWindow.GetSportsman();

                    // Проверяем, что основной вид спорта выбран
                    if (sportsman.IdSport_1 == 0)
                    {
                        MessageBox.Show("Выберите основной вид спорта");
                        return;
                    }

                    db.Sportman.Add(sportsman);
                    db.SaveChanges();
                    LoadSportsmen();
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
                        try
                        {
                            var updatedSportsman = editWindow.GetSportsman();

                            // Обновляем поля
                            sportsman.Name = updatedSportsman.Name;
                            sportsman.Team = updatedSportsman.Team;
                            sportsman.Lvl = updatedSportsman.Lvl;
                            sportsman.Date = updatedSportsman.Date;

                            // Обновляем виды спорта
                            sportsman.IdSport_1 = updatedSportsman.IdSport_1;
                            sportsman.IdSport_2 = updatedSportsman.IdSport_2;
                            sportsman.IdSport_3 = updatedSportsman.IdSport_3;
                            sportsman.IdSport_4 = updatedSportsman.IdSport_4;
                            sportsman.IdSport_5 = updatedSportsman.IdSport_5;

                            db.SaveChanges();
                            LoadSportsmen();
                            MessageBox.Show("Спортсмен успешно обновлен");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
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
                        "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.Sportman.Remove(sportsman);
                            db.SaveChanges();
                            LoadSportsmen();
                            MessageBox.Show("Спортсмен удален");
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

    // ViewModel для отображения спортсменов
    public class SportsmanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Lvl { get; set; }
        public DateTime Date { get; set; }
        public List<string> SportsList { get; set; }
    }
}

