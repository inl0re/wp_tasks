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
using System.Collections.ObjectModel;

namespace SList
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.LoadData();
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
            var itemAddBox = (TextBox)sender;
            var currentPivot = (Pivots)MyPivot.SelectedItem;
            if (e.Key.Equals(Key.Enter))
            {
                if (itemAddBox.Text != null)
                {
                    // Добавить элемент в начало коллекции
                    currentPivot.Items.Insert(0, (new ItemViewModel() { Name = itemAddBox.Text, ToDelete = "Collapsed" }));
                }
                itemAddBox.Text = "";                
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
                itemViewModel.ToDelete = "Visible";
            }
            // Убираем зачёркивание
            else if (itemViewModel.ToDelete == "Visible")
            {
                itemViewModel.ToDelete = "Collapsed";
            }
        }

        private void AddIconButton_Click(object sender, EventArgs e)
        {
            SearchInVisualTree(MyPivot);
        }

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

                    if (oTextBox.Tag.Equals(currentPivot.Title))
                    {
                        App.ViewModel.NewPivot(oTextBox.Text);
                        foreach (var pivot in App.ViewModel.PivotsList)
                        {
                            if (pivot.Title == oTextBox.Text)
                                MyPivot.SelectedItem = pivot;
                        }
                        return;
                    }
                }
                else
                {
                    SearchInVisualTree(child);
                }
            }
        }

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
                        App.ViewModel.PivotsList.RemoveAt(i);
                }
            }
        }

    }
}