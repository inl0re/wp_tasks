﻿using System;
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

namespace SList
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();          
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
            // Делаем список текущим при нажатии на тайл
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
                if (itemAddBox.Text.Length > 25)
                {
                    MessageBox.Show("Пункт списка не может быть длинее 25 символов");
                    return;
                }
                {
                    Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
                    // Добавить элемент в начало коллекции
                    currentPivot.Items.Insert(0, (new ItemViewModel() { Name = itemAddBox.Text, ToDelete = "Collapsed" }));
                    itemAddBox.Text = "";
                }
            }
        }

        // Нажатие на элемент списка
        private void ItemsTextBlock_Tap(object sender, GestureEventArgs e)
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

        // Добавить новый список
        private void AddIconButton_Click(object sender, EventArgs e)
        {
            SearchInVisualTree(MyPivot);
        }

        // Поиск по pivot'ам для добавления списка
        private void SearchInVisualTree(DependencyObject targetElement)
        {
            Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(targetElement); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(targetElement, i);
                if (child is TextBox)
                {
                    TextBox oTextBox = (TextBox)child;
                    // Проверка, не пустое ли название нового списка
                    if (string.IsNullOrWhiteSpace(oTextBox.Text))
                        return;

                    if (oTextBox.Text.Length > 16)
                    {
                        MessageBox.Show("Название списка не может быть длинее 16 символов");
                        return;
                    }
                        if (oTextBox.Tag.Equals(currentPivot.Title))
                        {
                            // Создание нового списка
                            App.ViewModel.NewPivot(oTextBox.Text);
                            Pivots pivot = App.ViewModel.PivotsList.First(p => p.Title == oTextBox.Text);
                            MyPivot.SelectedItem = pivot;
                            oTextBox.Text = "";
                            // Фокус на MainPage, чтобы убрать клавиатуру
                            this.Focus();
                            return;
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
                Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
                IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                // Удаляем файл списка из хранилища
                fileStorage.DeleteFile("Data\\" + currentPivot.Title);                
                // Ищем и удаляем список из коллекции
                Pivots pivot = App.ViewModel.PivotsList.First(p => p.Title == currentPivot.Title);
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
                // Если списков больше нет, переходим на стартовую 
                if (App.ViewModel.PivotsList.Count() == 0)
                {
                    NavigationService.Navigate(new Uri("/StartPage.xaml?from=MainPage", UriKind.Relative));
                }
            }
        }

        // Добавить Live Tile
        private void AddTile_Click(object sender, EventArgs e)
        {
            Pivots currentPivot = (Pivots)MyPivot.SelectedItem;
            Uri navUri = new Uri("/MainPage.xaml?title=" + currentPivot.Title, UriKind.Relative);
            if (ShellTile.ActiveTiles.Any(t => t.NavigationUri == navUri))
            {
                MessageBox.Show("Тайл уже закреплён");
                return;
            }
            App.ViewModel.TileAdd(currentPivot, false);
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); 
        }

    }
}