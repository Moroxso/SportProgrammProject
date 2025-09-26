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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SportProgramm;
using System.Data.Entity;

namespace SportProgramm.Pages.AdminPanelPages
{
    /// <summary>
    /// Логика взаимодействия для EditSportsPage.xaml
    /// </summary>
    public partial class EditSportsPage : Page
    {
        private SportProgrammProjectEntities db = new SportProgrammProjectEntities();

        public EditSportsPage()
        {
            InitializeComponent();
            LoadSports();
        }

        private void LoadSports()
        {
            try
            {
                db.Sports.Load();
                SportsList.ItemsSource = db.Sports.Local.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

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
                    LoadSports();
                    MessageBox.Show("Вид спорта добавлен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void EditSport_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int sportId = (int)button.Tag;
                var sport = db.Sports.Find(sportId);
                if (sport != null)
                {
                    var editWindow = new AddEditSportWindow(sport);
                    if (editWindow.ShowDialog() == true)
                    {
                        try
                        {
                            var updatedSport = editWindow.GetSport();
                            sport.Name = updatedSport.Name;
                            sport.Unit = updatedSport.Unit;
                            sport.Record = updatedSport.Record;
                            sport.Date = updatedSport.Date;
                            db.SaveChanges();
                            LoadSports();
                            MessageBox.Show("Вид спорта обновлен");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}");
                        }
                    }
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
                        "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.Sports.Remove(sport);
                            db.SaveChanges();
                            LoadSports();
                            MessageBox.Show("Вид спорта удален");
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
