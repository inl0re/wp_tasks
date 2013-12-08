using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SList
{
    public partial class MainPage : PhoneApplicationPage
    {    
        // Конструктор
        public MainPage()
        {
            InitializeComponent();            
        }

        public void ArrowHideShow()
        {
            bool isPivotsEmpty = App.ViewModel.IsPivotsEmpty();
            if (isPivotsEmpty == true)
                ImgArrow.Visibility = Visibility.Visible;
            else
                ImgArrow.Visibility = Visibility.Collapsed;
        }

        // Загрузка данных
        private void DataLoad()
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            } 
        }
       
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DataContext = App.ViewModel;
            DataLoad();
            string tileTitle;
            // Проверяем наличие списков
            ArrowHideShow();
            // Делаем список текущим, если пришли по нажатию на тайл
            if (NavigationContext.QueryString.TryGetValue("title", out tileTitle))
            {
                Pivots pivot = App.ViewModel.PivotsList.First(p => p.Title == tileTitle);
                MyPivot.SelectedItem = pivot;            
            }
            base.OnNavigatedTo(e);
        }
         
        // Добавление элементов в список по нажатию Enter
        private void ItemAddBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                TextBox itemAddBox = (TextBox)sender;
                // Проверка, не пустое ли название нового элемента списка
                if (string.IsNullOrWhiteSpace(itemAddBox.Text))
                    return;
                {
                    Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
                    // Добавить элемент в начало коллекции
                    currentPivot.Items.Insert(0, (new ItemViewModel() { Name = itemAddBox.Text, ToDelete = "Collapsed" }));
                    itemAddBox.Text = "";
                }
            }
        }

        private void ItemsTextBlock_Tap(object sender, GestureEventArgs e) // Зачёркивание пунктов
        {
            TextBlock txtBlk = (TextBlock)sender;
            ItemViewModel item = (ItemViewModel)txtBlk.DataContext;
            if (item.ToDelete == "Collapsed")
            {
                item.ToDelete = "Visible";
            }

            else if (item.ToDelete == "Visible")
            {
                item.ToDelete = "Collapsed";
            }

        }

        private void AddIconButton_Click(object sender, EventArgs e) // Добавить новый список
        {
            // переход на AddPage
            NavigationService.Navigate(new Uri("/AddPage.xaml", UriKind.Relative));
        }

        private void DeleteCurrentList_Click(object sender, EventArgs e) // Удаление текущего списка
        {
            Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
            if (currentPivot != null)
            {
                MessageBoxResult m = MessageBox.Show("Вы точно хотите безвозвратно удалить список?", "Удалить список", MessageBoxButton.OKCancel);
                if (m == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (m == MessageBoxResult.OK)
                {

                    IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    fileStorage.DeleteFile("Data\\" + currentPivot.Title); // Удаляем файл списка из хранилища                
                    Pivots pivot = App.ViewModel.PivotsList.First(p => p.Title == currentPivot.Title); // Ищем и удаляем список из коллекции
                    App.ViewModel.PivotsList.Remove(pivot);
                    // Удаляем картинку тайла из хранилища
                    try
                    {
                        fileStorage.DeleteFile(string.Format("/Shared/ShellContent/{0}.png", currentPivot.Title));
                    }
                    catch
                    {
                        // do nothing
                    }
                    // Удаляем соответсвующий тайл
                    Uri navUri = new Uri("/MainPage.xaml?title=" + currentPivot.Title, UriKind.Relative);
                    ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri == navUri);
                    if (tile != null)
                        tile.Delete();
                }
            }
            ArrowHideShow();
        }

        private void AddTile_Click(object sender, EventArgs e) // Добавить тайл
        {
            Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
            if (currentPivot != null)
            {
                Uri navUri = new Uri("/MainPage.xaml?title=" + currentPivot.Title, UriKind.Relative);
                if (ShellTile.ActiveTiles.Any(t => t.NavigationUri == navUri))
                {
                    MessageBox.Show("Тайл уже закреплён");
                    return;
                }
                App.tile.AddTile(currentPivot, false);
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); 
        }

    }
}