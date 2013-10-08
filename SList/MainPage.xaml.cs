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
            if (e.Key.Equals(Key.Enter))
            {
                if (ItemAddBox.Text != null)
                {
                    var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    var fileWrite = new StreamWriter(new IsolatedStorageFileStream("List.txt", FileMode.Append, fileStorage));
                    fileWrite.WriteLine(ItemAddBox.Text);
                    fileWrite.Close();

                    App.ViewModel.Items.Insert(0, (new ItemViewModel() { Name = ItemAddBox.Text, ToDelete = "Collapsed" }));
                    
                }
                ItemAddBox.Text = "";
                // this.Focus();
            }
        }

        private void ItemsTextBlock_Tap(object sender, GestureEventArgs e)
        {
            var txtBlk = (TextBlock)sender;
            var itemViewModel = (ItemViewModel)txtBlk.DataContext;
            if (itemViewModel.ToDelete == "Collapsed")
            {
                txtBlk.Opacity = 0.7;
                itemViewModel.ToDelete = "Visible";



                /*
                string line = null;
                var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                var fileRead = new StreamReader(new IsolatedStorageFileStream("List.txt", FileMode.OpenOrCreate, fileStorage));
                // Открываем на запись временный файл
                var fileWrite = new StreamWriter(new IsolatedStorageFileStream("Temp.txt", FileMode.OpenOrCreate, fileStorage));
                // Перебираем в цикле строки и пишем в файл, выкинув удалённую
                while ((line = fileRead.ReadLine()) != null)
                {
                    if (String.Compare(line, itemViewModel.Name) == 0)
                        continue;
                    fileWrite.WriteLine(line);
                }
                fileWrite.Close();
                fileRead.Close();
                // Удаляем основной файл, затем временный копируем на его место
                fileStorage.DeleteFile("List.txt");
                fileStorage.MoveFile("Temp.txt", "List.txt");     
                */
            }

            else if (itemViewModel.ToDelete == "Visible")
            {
                /*
                var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                var fileWrite = new StreamWriter(new IsolatedStorageFileStream("List.txt", FileMode.Append, fileStorage));
                fileWrite.WriteLine(txtBlk.Text);
                fileWrite.Close();
                */
                txtBlk.Opacity = 1;
                itemViewModel.ToDelete = "Collapsed";
            }

            // Проверяем, есть ли зачёркнутые пункты, для появления ApplicationBar
            foreach (var item in App.ViewModel.Items)
            {
                if (item.ToDelete == "Visible")
                {
                    ApplicationBar.IsVisible = true;
                    break;
                }
                else
                    ApplicationBar.IsVisible = false;
            }
        }

        // Удалить вычеркнутые
        private void RefreshIconButton_Click(object sender, EventArgs e)
        {
            foreach (var item in App.ViewModel.Items.ToList())
            {
                if (item.ToDelete == "Visible")
                    App.ViewModel.Items.Remove(item);
            }
            /*
            App.ViewModel.Items.Clear();
            App.ViewModel.LoadData();
             */
            ApplicationBar.IsVisible = false;
        }

        // Тестовая кнопка
        private void DebugIconButton_Click(object sender, EventArgs e)
        {
            /* for (int i = 0; i < App.ViewModel.Items.Count(); i++)
            {
                if (App.ViewModel.Items[i].ToDelete == "Visible")
                {
                    ApplicationBar.ToDelete = true;
                    break;
                }
            }
             */
            foreach (var item in App.ViewModel.Items)
            {
                if (item.ToDelete == "Visible")
                {
                    ApplicationBar.IsVisible = true;
                    break;
                }
            }
        }
        
    }
}