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
using D1.Data;

namespace D1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Material> MaterialList { get; set; }

        public List<Material> FilteredMaterialList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MaterialList = ReadFromDatabase.GetAllMaterials();
            FilteredMaterialList = MaterialList;

            MaterialListView.ItemsSource = MaterialList;

            DataContext = this;

        }
    }
}
