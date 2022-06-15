using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using D1.Data;
using Microsoft.Win32;

namespace D1.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditMaterialWindow.xaml
    /// </summary>
    public partial class AddEditMaterialWindow : Window
    {

        public Material SelectedMaterial { get; set; }

        public ObservableCollection<string> MaterialSupplierNames { get; set; }

        private string MaterialImageName;


        public AddEditMaterialWindow()
        {
            InitializeComponent();

            MaterialTypeComboBox.ItemsSource = DatabaseOperations.GetAllMaterialTypes().Select(u => u.Title);
            MaterialUnitComboBox.ItemsSource = DatabaseOperations.GetAllUnits().Select(u => u.Name);
            SuppliersComboBox.ItemsSource = DatabaseOperations.GetAllSuppliers().Select(s => s.Title);

            MaterialSupplierNames = new ObservableCollection<string>();
            SupplierListView.ItemsSource = MaterialSupplierNames;

        }


        public AddEditMaterialWindow(Material selectedMaterial) : this()
        {

            TitleTextBlock.Text = "Редактирование материала";
            AddEditMaterialButton.Content = "Изменить материал";
            DeleteMaterialButton.Visibility = Visibility.Visible;


            SelectedMaterial = selectedMaterial;
            List<string> SelectedMaterialSupplierNames = SelectedMaterial.Supplier.Select(s => s.Title).ToList();

            foreach (var supplierName in SelectedMaterialSupplierNames)
            {
                MaterialSupplierNames.Add(supplierName);
            }

            DataContext = SelectedMaterial;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedMaterial is null)
            {
                MaterialImage.Source = new BitmapImage(new Uri("/Image/picture.png", UriKind.Relative));
            }
        }

        private void PriceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (InputValidation.ValidatePrice(e))
                e.Handled = true;
        }


        private void NumberTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            char character = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            if (InputValidation.ValidateDigit(e))
                e.Handled = true;

        }

        private void AddSelectedSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierListView.Items.Contains(SuppliersComboBox.Text))
                return;

            MaterialSupplierNames.Add(SuppliersComboBox.Text);

        }

        private void RemoveSelectedSupplierButton_Click(object sender, RoutedEventArgs e)
        {

            if (SupplierListView.SelectedItem is null)
                return;

            MaterialSupplierNames.RemoveAt(SupplierListView.SelectedIndex);

        }

        private void CalculatePriceButton_Click(object sender, RoutedEventArgs e)
        {

            if (MinCountTextBox.Text.Equals(string.Empty)
                || CountInStockTextBox.Equals(string.Empty)
                || CountInPackTextBox.Equals(string.Empty)
                || PriceTextBox.Equals(string.Empty))
            {
                MessageBox.Show("Введены не все необходимые значения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int CountInStock = Convert.ToInt32(CountInStockTextBox.Text);
            int MinCount = Convert.ToInt32(MinCountTextBox.Text);

            if (CountInStock > MinCount)
            {
                MessageBox.Show("Количество на складе уже превышает минимальное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double Difference = MinCount - CountInStock;
            double CountInPack = Convert.ToDouble(CountInPackTextBox.Text);

            decimal PackagesNeeded = (decimal)Math.Ceiling(Difference / CountInPack);
            decimal PricePerItem = Convert.ToDecimal(PriceTextBox.Text.Replace('.', ','));


            decimal Price = PackagesNeeded * (decimal)CountInPack * PricePerItem;
            MessageBox.Show($"Стоимость минимальной необходимой партии: {Price}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image (*.png);(*.jpg);(*.jpeg) | *.png;*.jpg;*.jpeg;";


            if (fileDialog.ShowDialog() == true)
            {
                string path = fileDialog.FileName;

                File.Copy(path, Directory.GetCurrentDirectory() + $@"\materials\{Path.GetFileName(path)}", true);

                MaterialImageName = $@"\materials\" + Path.GetFileName(path);

                MaterialImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));


            }


        }

        private void AddEditMaterialButton_Click(object sender, RoutedEventArgs e)
        {

            string ErrorMessage = "";

            if (NameTextBox.Text.Equals(string.Empty))
                ErrorMessage += "Не введено имя материала \n";

            if (CountInStockTextBox.Text.Equals(string.Empty))
                ErrorMessage += "Не введено количество материала на складе \n";

            if (CountInPackTextBox.Text.Equals(string.Empty))
                ErrorMessage += "Не введено количество материала в упаковке \n";

            if (MinCountTextBox.Text.Equals(string.Empty))
                ErrorMessage += "Не введено минимальное количество материала \n";

            if (PriceTextBox.Text.Equals(string.Empty))
                ErrorMessage += "Не введена стоимость материала \n";


            if (ErrorMessage.Length != 0)
            {
                MessageBox.Show("Ошибка: \n" + ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedMaterial is null)
            {
                AddMaterial();
            }
            else
            {
                EditMaterial();
            }

        }


        private void EditMaterial()
        {

            DatabaseOperations.SaveEditedMaterial(FillMaterial(SelectedMaterial), MaterialSupplierNames.ToList());


            MessageBox.Show("Материал успешно изменён", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();

        }

        private void AddMaterial()
        {

            Material NewMaterial = FillMaterial(new Material());

            DatabaseOperations.AddNewMaterial(NewMaterial, MaterialSupplierNames.ToList());

            MessageBox.Show("Материал успешно добавлен", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);


            this.Close();

        }

        private Material FillMaterial(Material material)
        {

            material.Title = NameTextBox.Text;
            material.MaterialTypeID = MaterialTypeComboBox.SelectedIndex + 1;
            material.CountInStock = Convert.ToInt32(CountInStockTextBox.Text);
            material.UnitID = MaterialUnitComboBox.SelectedIndex + 1;
            material.Description = DescriptionTextBox.Text;
            material.CountInPack = Convert.ToInt32(CountInPackTextBox.Text);
            material.MinCount = Convert.ToInt32(MinCountTextBox.Text);
            material.Cost = Convert.ToDecimal(PriceTextBox.Text.Replace('.', ','));
            material.Image = MaterialImageName;



            return material;
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            string AllowedSymbols = "1234567890,";


            if ((e.Text == "," && PriceTextBox.Text.Equals(string.Empty)) || (PriceTextBox.Text.Contains(',') && e.Text == ","))
                e.Handled = true;



            if (!e.Text.Any(t => AllowedSymbols.Contains(t)))
                e.Handled = true;


        }

        private void DeleteMaterialButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
