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
using D1.Data;


namespace D1.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeMInimalAmountWindow.xaml
    /// </summary>
    public partial class ChangeMInimalAmountWindow : Window
    {

        public List<Material> MaterialList { get; set; }

        public ChangeMInimalAmountWindow(List<int> selectedItemsId)
        {
            InitializeComponent();


            MaterialList = DatabaseOperations.GetMaterialsByID(selectedItemsId);


            MinCountTextBox.Text = MaterialList.Max(m => m.MinCount).ToString();

        }

        private void ChangeMinCountButton_Click(object sender, RoutedEventArgs e)
        {

            if (MinCountTextBox.Text.Equals(string.Empty))
            {
                MessageBox.Show("Должно быть введено новое минммальное количество", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


            DatabaseOperations.ChangeMaterialsMinCount(MaterialList, Convert.ToInt32(MinCountTextBox.Text));


            MessageBox.Show("Минимальное количество успешно изменено", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();


        }

        private void MinCountTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
            if (InputValidation.ValidateDigit(e))
                e.Handled = true;


        }
    }
}
