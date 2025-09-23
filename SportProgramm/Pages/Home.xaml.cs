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
            LoadTournaments();
        }

        private void LoadTournaments()
        {
            try
            {
                db.Cup.Load();
                TournamentsGrid.ItemsSource = db.Cup.Local.ToBindingList();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }



    }
}
