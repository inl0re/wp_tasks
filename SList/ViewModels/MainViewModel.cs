using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;


namespace SList
{
    // Коллекция для элементов списка
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.PivotsList = new ObservableCollection<Pivots>();
        }

        /// <summary>
        /// Список Pivots.
        /// </summary>
        public ObservableCollection<Pivots> PivotsList { get; private set; }



        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Создает и добавляет несколько объектов Pivots в коллекцию элементов.
        /// </summary>
        public void LoadData()
        {
           var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
           string[] fileNames = fileStorage.GetFileNames();
           if (fileNames.Length > 0)
           {
               for (int f = 0; f < fileNames.Length; f++)
               {
                   var fileRead = new StreamReader(new IsolatedStorageFileStream(fileNames[f], FileMode.OpenOrCreate, fileStorage));
                   string list = fileRead.ReadToEnd();
                   string[] lines = list.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                   this.PivotsList.Add(new Pivots() { Title = fileNames[f], Items = new ObservableCollection<ItemViewModel>() });
                   if (lines.Length > 0)
                   {
                       for (int i = 0; i < lines.Length; i++)
                       {
                           this.PivotsList[f].Items.Add(new ItemViewModel() { Name = lines[i], ToDelete = "Collapsed" });
                       }
                   }
                   fileRead.Close();
               }

               /*
               this.PivotsList.Add(new Pivots() { Title = "фрукты", Items = new ObservableCollection<ItemViewModel>() });
               this.PivotsList[1].Items.Add(new ItemViewModel() { Name = "яблоки", ToDelete = "Collapsed" });
               this.PivotsList[1].Items.Add(new ItemViewModel() { Name = "груши", ToDelete = "Collapsed" });
                */
               this.IsDataLoaded = true;
           }
           else
           {
               this.PivotsList.Add(new Pivots() { Title = "заглушка", Items = new ObservableCollection<ItemViewModel>() });
               this.IsDataLoaded = true;
           }
        }

        public void NewPivot(string title)
        {
            this.PivotsList.Add(new Pivots() { Title = title, Items = new ObservableCollection<ItemViewModel>() });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}