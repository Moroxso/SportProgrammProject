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
    /// Логика взаимодействия для AddEditSportWindow.xaml
    /// </summary>
    public partial class AddEditSportWindow : Window
    {
        private Sports _sport;

        public AddEditSportWindow(Sports sport = null)
        {
            InitializeComponent();
            _sport = sport;

            if (_sport != null)
            {
                Title = "Редактировать вид спорта";
                NameTextBox.Text = _sport.Name;
                UnitTextBox.Text = _sport.Unit;
                RecordTextBox.Text = _sport.Record;

                // Исправление для DateTime
                if (_sport.Date != null) // Проверка на null вместо HasValue
                {
                    try
                    {
                        // Пробуем распарсить дату из строки
                        if (DateTime.TryParse(_sport.Date.ToString(), out DateTime date))
                        {
                            DatePicker.SelectedDate = date;
                        }
                    }
                    catch
                    {
                        // Если не удалось распарсить, оставляем пустым
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название вида спорта");
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

        public Sports GetSport()
        {
            var sport = _sport ?? new Sports();
            sport.Name = NameTextBox.Text.Trim();
            sport.Unit = UnitTextBox.Text.Trim();
            sport.Record = RecordTextBox.Text.Trim();

            // Исправление для DateTime
            sport.Date = DatePicker.SelectedDate?.ToString("dd.MM.yyyy"); // Преобразуем в строку

            return sport;
        }
    }
}