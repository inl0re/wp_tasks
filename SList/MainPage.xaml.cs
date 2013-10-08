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
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;

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

        // Добавление в список по нажатию клавиши Enter
        private void ItemAddBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Если нажат Enter
            if (e.Key.Equals(Key.Enter))
            {
                if (ItemAddBox.Text != null)
                {
                    var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    var fileWrite = new StreamWriter(new IsolatedStorageFileStream("List.txt", FileMode.Append, fileStorage));
                    fileWrite.WriteLine(ItemAddBox.Text);
                    fileWrite.Close();
                    // Добавить элемент в начало коллекции
                    App.ViewModel.Items.Insert(0, (new ItemViewModel() { Name = ItemAddBox.Text, ToDelete = "Collapsed" }));                 
                }
                // Очистить текстбокс
                ItemAddBox.Text = "";
            }
        }

        // Нажатие на элемент списка
        private void ItemsTextBlock_Tap(object sender, GestureEventArgs e)
        {
            var txtBlk = (TextBlock)sender;
            var itemViewModel = (ItemViewModel)txtBlk.DataContext;
            // Зачёркиваем элемент
            if (itemViewModel.ToDelete == "Collapsed")
            {
                txtBlk.Opacity = 0.7;
                itemViewModel.ToDelete = "Visible";
            }
            // Убираем зачёркивание
            else if (itemViewModel.ToDelete == "Visible")
            {
                txtBlk.Opacity = 1;
                itemViewModel.ToDelete = "Collapsed";
            }
        }     
    }
}