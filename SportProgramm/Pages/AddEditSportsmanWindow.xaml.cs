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
    /// Логика взаимодействия для AddEditSportsmanWindow.xaml
    /// </summary>
    public partial class AddEditSportsmanWindow : Window
    {
        private Sportman _sportsman;
        private List<Sports> _sports;

        public AddEditSportsmanWindow(List<Sports> sports, Sportman sportsman = null)
        {
            InitializeComponent();
            _sportsman = sportsman;
            _sports = sports;

            // Заполняем комбобоксы
            var comboboxes = new[] { Sport1ComboBox, Sport2ComboBox, Sport3ComboBox, Sport4ComboBox, Sport5ComboBox };
            foreach (var cb in comboboxes)
            {
                cb.ItemsSource = _sports;
            }

            if (_sportsman != null)
            {
                Title = "Редактировать спортсмена";
                NameTextBox.Text = _sportsman.Name;
                TeamTextBox.Text = _sportsman.Team;
                LvlTextBox.Text = _sportsman.Lvl;

                // Для DateTime просто присваиваем
                DatePicker.SelectedDate = _sportsman.Date;

                // Заполняем выбранные виды спорта
                SetSelectedSport(Sport1ComboBox, _sportsman.IdSport_1);
                SetSelectedSport(Sport2ComboBox, _sportsman.IdSport_2 ?? int.MinValue);
                SetSelectedSport(Sport3ComboBox, _sportsman.IdSport_3 ?? int.MinValue);
                SetSelectedSport(Sport4ComboBox, _sportsman.IdSport_4 ?? int.MinValue);
                SetSelectedSport(Sport5ComboBox, _sportsman.IdSport_5 ?? int.MinValue);
            }
        }

        private void SetSelectedSport(ComboBox comboBox, int sportId)
        {
            if (sportId > 0)
            {
                comboBox.SelectedItem = _sports.FirstOrDefault(s => s.Id == sportId);
            }
        }

        private int GetSelectedSportId(ComboBox comboBox)
        {
            return comboBox.SelectedItem is Sports sport ? sport.Id : 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TeamTextBox.Text) ||
                string.IsNullOrWhiteSpace(LvlTextBox.Text))
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

        public Sportman GetSportsman()
        {
            var sportsman = _sportsman ?? new Sportman();
            sportsman.Name = NameTextBox.Text.Trim();
            sportsman.Team = TeamTextBox.Text.Trim();
            sportsman.Lvl = LvlTextBox.Text.Trim();

            // Для DateTime используем значение или текущую дату
            sportsman.Date = DatePicker.SelectedDate ?? DateTime.Now;

            sportsman.IdSport_1 = GetSelectedSportId(Sport1ComboBox);
            sportsman.IdSport_2 = GetSelectedSportId(Sport2ComboBox);
            sportsman.IdSport_3 = GetSelectedSportId(Sport3ComboBox);
            sportsman.IdSport_4 = GetSelectedSportId(Sport4ComboBox);
            sportsman.IdSport_5 = GetSelectedSportId(Sport5ComboBox);

            return sportsman;
        }
    }
}