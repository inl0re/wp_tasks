using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using ProTile.Lib;
using ProTile;

namespace SList
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Загрузка данных для элементов ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            
        }

        // При нажатии на тайл списка делаем этот список активным в приложении
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string tileTitle;
            if (NavigationContext.QueryString.TryGetValue("title", out tileTitle))
            {
                foreach (var pivot in App.ViewModel.PivotsList)
                {
                    if (pivot.Title == tileTitle)
                    {
                        MyPivot.SelectedItem = pivot;
                        return;
                    }
                }               
            }
            base.OnNavigatedTo(e);
        }

        // Добавление элементов в список по нажатию клавиши Enter
        private void ItemAddBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                var itemAddBox = (TextBox)sender;
                // Проверка, не пустое ли название нового элемента списка
                if (!string.IsNullOrWhiteSpace(itemAddBox.Text) && itemAddBox.Text.Length < 28)
                {
                    var currentPivot = (Pivots)MyPivot.SelectedItem;
                    // Добавить элемент в начало коллекции
                    currentPivot.Items.Insert(0, (new ItemViewModel() { Name = itemAddBox.Text, ToDelete = "Collapsed" }));
                    itemAddBox.Text = "";
                }
            }
        }

        // Нажатие на элемент списка
        private void ItemsTextBlock_Tap(object sender, GestureEventArgs e)
        {
            var txtBlk = (TextBlock)sender;
            var itemViewModel = (ItemViewModel)txtBlk.DataContext;
            if (itemViewModel.ToDelete == "Collapsed")
            {
                itemViewModel.ToDelete = "Visible";
            }

            else if (itemViewModel.ToDelete == "Visible")
            {
                itemViewModel.ToDelete = "Collapsed";
            }

        }

        // Добавить новый список
        private void AddIconButton_Click(object sender, EventArgs e)
        {
            SearchInVisualTree(MyPivot);
        }

        // Поиск по pivot'ам
        private void SearchInVisualTree(DependencyObject targetElement)
        {
            var currentPivot = (Pivots)MyPivot.SelectedItem;
            var oCount = VisualTreeHelper.GetChildrenCount(targetElement);
            for (int i = 0; i < oCount; i++)
            {
                var child = VisualTreeHelper.GetChild(targetElement, i);
                if (child is TextBox)
                {
                    TextBox oTextBox = (TextBox)child;
                    // Проверка, не пустое ли название нового списка
                    if (!string.IsNullOrWhiteSpace(oTextBox.Text) && oTextBox.Text.Length < 28)
                    {
                        if (oTextBox.Tag.Equals(currentPivot.Title))
                        {
                            // Создание нового списка
                            App.ViewModel.NewPivot(oTextBox.Text);                            
                            foreach (var pivot in App.ViewModel.PivotsList)
                            {
                                // Переключение на новый список
                                if (pivot.Title == oTextBox.Text)
                                    MyPivot.SelectedItem = pivot;
                            }
                            oTextBox.Text = "";
                            // Фокус на MainPage, чтобы убрать клавиатуру
                            this.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    SearchInVisualTree(child);
                }
            }
        }

        // Удаление текущего списка
        private void DeleteCurrentList_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Вы точно хотите безвозвратно удалить список?", "Удалить список", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.Cancel)
            {
                return;
            }
            else if (m == MessageBoxResult.OK)
            {
                var currentPivot = (Pivots)MyPivot.SelectedItem;
                var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                fileStorage.DeleteFile(currentPivot.Title);
                for (int i = 0; i < App.ViewModel.PivotsList.Count(); i++)
                {
                    if (App.ViewModel.PivotsList[i].Title == currentPivot.Title)
                    {
                        App.ViewModel.PivotsList.RemoveAt(i);
                        if (App.ViewModel.PivotsList.Count() == 0)
                            NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
                    }
                }
            }
        }

        // Добавить Live Tile на раб. стол
        private void AddTile_Click(object sender, EventArgs e)
        {            
            var currentPivot = (Pivots)MyPivot.SelectedItem;
            // ShellTile SecondaryTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(currentPivot.Title));
            var navUri = new Uri("/MainPage.xaml?title=" + currentPivot.Title, UriKind.Relative);
            if (ShellTile.ActiveTiles.Any(t => t.NavigationUri == navUri))
            {
                MessageBox.Show("Tile is already pinned");
                return;
            }

            string fileName = navUri.ToString().GetHashCode().ToString();
            string list = "";
            foreach (var item in currentPivot.Items)
            {
                list += item.Name + "\r\n";
            }

            // generate image for the front tile
            Tile tile = new Tile
            {
                description = { Text = list },
            };
            SaveTile(tile, string.Format("/Shared/ShellContent/{0}.png", fileName));

            // generate image for the back tile
            /*
            TileBack tileBack = new TileBack
            {
                text = { Text = "message" },
                number = { Text = "number" },
            };
            SaveTile(tileBack, string.Format("/Shared/ShellContent/{0}back.png", fileName));
             */

            StandardTileData newTileData = new StandardTileData
            {
                Title = string.Empty,
                // use proper address pointing to isolated storage!
                BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}.png", fileName)),
                // BackBackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}back.png", fileName)),
            };
            // Create the tile and pin it to Start.
            ShellTile.Create(navUri, newTileData);

            /*
            StandardTileData data = new StandardTileData
            {
                Title = currentPivot.Title
            };
            ShellTile.Create(navUri, data);   
             */
        }

        public void SaveTile(UserControl tile, string fileName)
        {
            // call Measure and Arrange because Tile is not part of logical tree
            tile.Measure(new Size(173, 173));
            tile.Arrange(new Rect(0, 0, 173, 173));

            // render the Tile into WriteableBitmap
            WriteableBitmap tileImage = new WriteableBitmap(173, 173);
            tileImage.Render(tile, null);
            tileImage.Invalidate();

            // save is as Png file
            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().CreateFile(fileName))
            {
                tileImage.SavePng(stream);
            }
        }
    }
}