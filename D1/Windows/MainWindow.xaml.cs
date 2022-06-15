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
using D1.Windows;
using D1.Data;

namespace D1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<MaterialWithSuppliers> MaterialList { get; set; }

        public List<MaterialWithSuppliers> FilteredMaterialList { get; set; }

        private int _CurrentPageIndex = 0;

        private int ItemsPerPage = 15;

        private string CurrentSorting = "По умолчанию";

        private string CurrentFilter = "Все типы";

        private string CurrentSearch = "";


        public MainWindow()
        {

            DatabaseOperations.InitializeEntities();

            MaterialList = DatabaseOperations.GetAllMaterials();
            FilteredMaterialList = MaterialList;

            InitializeComponent();

            List<MaterialType> MaterialTypeList = DatabaseOperations.GetAllMaterialTypes();
            FilterComboBox.Items.Add("Все типы");
            foreach (var materialType in MaterialTypeList)
            {
                FilterComboBox.Items.Add(materialType.Title);
            }
            
            DataContext = this;

        }


        private void UpdateMaterialsFromDatabase()
        {
            MaterialList = DatabaseOperations.GetAllMaterials();
            
        }

        private void MaterialWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GeneratePageButtons();
            UpdateMaterialListView();
            ChangePage();
        }

        public void GeneratePageButtons()
        {
            if (ButtonStackPanel is null)
                return;

            ButtonStackPanel.Children.Clear();

            for (int i = 0; i < 5; i++)
            {

                if (ItemsPerPage * i > FilteredMaterialList.Count)
                    return;

                Button PageButton = new Button
                {
                    Width=50,
                    Height=50,
                    Content = (i + 1).ToString(),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    BorderBrush = Brushes.Black

                };

                if(i == 0)
                {
                    PageButton.BorderThickness = new Thickness(0, 0, 0, 5);
                }

                PageButton.Click += PageButton_Click;

                ButtonStackPanel.Children.Add(PageButton);
              
            }

            

        }

        private void PageLeftButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentPageIndex = _CurrentPageIndex == 0 ? 0 : --_CurrentPageIndex;
            ChangePage();
        }

        private void PageRightButton_Click(object sender, RoutedEventArgs e)
        {
            _CurrentPageIndex = (_CurrentPageIndex + 1) * ItemsPerPage > FilteredMaterialList.Count ? _CurrentPageIndex : ++_CurrentPageIndex;
            ChangePage();
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)            
        {
            _CurrentPageIndex = Convert.ToInt32((sender as Button).Content.ToString()) - 1;
            ChangePage();
        }

        private void ChangePage()
        {

            

            if (ButtonStackPanel is null || ButtonStackPanel.Children.Count == 0)
                return;

            List<Button> PageButtonList = ButtonStackPanel.Children.OfType<Button>().ToList();

            if(Convert.ToInt32(PageButtonList.Last().Content.ToString()) == _CurrentPageIndex)
            {
                foreach (var pageButton in PageButtonList)
                {
                    pageButton.Content = Convert.ToInt32(pageButton.Content.ToString()) + 1;
                }

            }

            if (Convert.ToInt32(PageButtonList.First().Content.ToString()) == _CurrentPageIndex + 1 && _CurrentPageIndex != 0)
            {
                foreach (var pageButton in PageButtonList)
                {
                    pageButton.Content = Convert.ToInt32(pageButton.Content.ToString()) - 1;
                }

            }


            foreach (var pageButton in PageButtonList)
            {
                pageButton.BorderThickness = new Thickness(0);


                if (pageButton.Content.ToString().Equals((_CurrentPageIndex + 1).ToString()))
                {
                    pageButton.BorderThickness = new Thickness(0, 0, 0, 5);
                    
                }


            }

            
            UpdateMaterialListView();
            
            

        }


        private void UpdateMaterialListView()
        {
            if (MaterialListView is null)
                return;


            FilterMaterials();

            MaterialListView.ItemsSource = FilteredMaterialList.Skip(_CurrentPageIndex * ItemsPerPage).Take(15);

            NoItemsFoundTextBlock.Visibility = FilteredMaterialList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            ItemsShownTextBlock.Text = $"Выведено {FilteredMaterialList.Count} из {MaterialList.Count}";

        }

        private void FilterMaterials()
        {

            var materialList = MaterialList?.Where(m => CurrentFilter == "Все типы" ? true : m.BaseMaterial.MaterialType.Title == CurrentFilter);

            materialList = materialList.Where(m => m.BaseMaterial.Title.Contains(CurrentSearch) 
            || m.BaseMaterial.Description?.Contains(CurrentSearch) == true);


            switch (CurrentSorting)
            {
                case "По умолчанию":
                    materialList = materialList.OrderBy(m => m.BaseMaterial.ID);
                    break;
                case "Наименование(по возрастанию)":
                    materialList = materialList.OrderBy(m => m.BaseMaterial.Title);
                    break;
                case "Наименование(по убыванию)":
                    materialList = materialList.OrderByDescending(m => m.BaseMaterial.Title);
                    break;
                case "Остаток на складе(по возрастанию)":
                    materialList = materialList.OrderBy(m => m.BaseMaterial.CountInStock);
                    break;
                case "Остаток на складе(по убыванию)":
                    materialList = materialList.OrderByDescending(m => m.BaseMaterial.CountInStock);
                    break;
                case "Стоимость(по возрастанию)":
                    materialList = materialList.OrderBy(m => m.BaseMaterial.Cost);
                    break;
                case "Стоимость(по убыванию)":
                    materialList = materialList.OrderByDescending(m => m.BaseMaterial.Cost);
                    break;

            }


            FilteredMaterialList = materialList.ToList();

          
        }

        
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSorting = (e.AddedItems[0] as TextBlock).Text;        
            ChangePage();
            GeneratePageButtons();

        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentFilter = e.AddedItems[0].ToString();           
            ChangePage();
            GeneratePageButtons();
        }
       

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentSearch = (sender as TextBox).Text;          
            ChangePage();
            GeneratePageButtons();
        }

        private void MaterialListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeMinimalAmountButton.Visibility = MaterialListView.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void ChangeMinimalAmountButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialListView.SelectedItems is null)
                return;

            List<int> selectedIdList = new List<int>();

            foreach (var item in MaterialListView.SelectedItems)
            {

                selectedIdList.Add((item as MaterialWithSuppliers).BaseMaterial.ID);

            }

            new ChangeMInimalAmountWindow(selectedIdList).ShowDialog();

            UpdateMaterialsFromDatabase();
            UpdateMaterialListView();

        }

        private void EditMaterialButton_Click(object sender, RoutedEventArgs e)
        {

            Material SelectedMaterial = DatabaseOperations.GetMaterialsByID(Convert.ToInt32((sender as Button).Tag));

            new AddEditMaterialWindow(SelectedMaterial).ShowDialog();

            UpdateMaterialsFromDatabase();
            UpdateMaterialListView();

        }

        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditMaterialWindow().ShowDialog();

            UpdateMaterialsFromDatabase();
            UpdateMaterialListView();
        }
    }
}
