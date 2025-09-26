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

namespace SportProgramm.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditTournamentWindow.xaml
    /// </summary>
    public partial class AddEditTournamentWindow : Window
    {
        private Cup _tournament;
        private List<Sports> _sports;

        public AddEditTournamentWindow(List<Sports> sports, Cup tournament = null)
        {
            InitializeComponent();
            _tournament = tournament;
            _sports = sports;

            SportComboBox.ItemsSource = _sports;

            if (_tournament != null)
            {
                Title = "Редактировать турнир";
                NameTextBox.Text = _tournament.Name;
                PlaceTextBox.Text = _tournament.Place;
                ScoreTextBox.Text = _tournament.Score;

                // Для DateTime просто присваиваем
                DatePicker.SelectedDate = _tournament.Date;

                if (_tournament.IdSport > 0)
                    SportComboBox.SelectedItem = _sports.FirstOrDefault(s => s.Id == _tournament.IdSport);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PlaceTextBox.Text) ||
                SportComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public Cup GetTournament()
        {
            var tournament = _tournament ?? new Cup();
            tournament.Name = NameTextBox.Text.Trim();
            tournament.Place = PlaceTextBox.Text.Trim();
            tournament.Score = ScoreTextBox.Text.Trim();

            // Для DateTime используем значение или текущую дату
            tournament.Date = DatePicker.SelectedDate ?? DateTime.Now;

            tournament.IdSport = ((Sports)SportComboBox.SelectedItem).Id;
            return tournament;
        }
    }
}
