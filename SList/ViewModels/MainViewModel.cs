using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;

namespace SList
{
    // Коллекция для элементов списка
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.PivotsList = new ObservableCollection<Pivots>();
        }

        public ObservableCollection<Pivots> PivotsList { get; private set; } // Коллекция списков

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData() // Загрузка списков
        {
           IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
           if (!fileStorage.DirectoryExists("Data"))
               fileStorage.CreateDirectory("Data");
           string[] fileNames = fileStorage.GetFileNames("Data\\*");
           if (fileNames.Length > 0)
           {
               for (int f = 0; f < fileNames.Length; f++)
               {
                   StreamReader fileRead = new StreamReader(new IsolatedStorageFileStream("Data\\" + fileNames[f], FileMode.OpenOrCreate, fileStorage));
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
               this.IsDataLoaded = true;
           }
           else
           {
               this.IsDataLoaded = true;
           }
        }

        public bool IsPivotsEmpty()
        {
            if (this.PivotsList.Count() > 0)
                return false;
            else
                return true;
        }

        public void AddPivot(string title) // Создаём новый список
        {
            if (this.PivotsList.Any(p => p.Title == title))
            {
                MessageBox.Show(string.Format("Список \"{0}\" уже существует", title));
                return;
            }
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