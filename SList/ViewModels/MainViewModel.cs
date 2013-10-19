using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Microsoft.Phone.Shell;
using ProTile.Lib;
using ProTile;


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
           IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
           string[] fileNames = fileStorage.GetFileNames();
           if (fileNames.Length > 0)
           {
               for (int f = 0; f < fileNames.Length; f++)
               {
                   StreamReader fileRead = new StreamReader(new IsolatedStorageFileStream(fileNames[f], FileMode.OpenOrCreate, fileStorage));
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

        public void NewPivot(string title)
        {
            if (this.PivotsList.Any(p => p.Title == title))
            {
                MessageBox.Show(string.Format("Список \"{0}\" уже существует", title));
                return;
            }
            this.PivotsList.Add(new Pivots() { Title = title, Items = new ObservableCollection<ItemViewModel>() });
        }

        public void TileAdd(Pivots pivot, bool update)
        {
            Uri navUri = new Uri("/MainPage.xaml?title=" + pivot.Title, UriKind.Relative);
            string fileName = pivot.Title;
            string list = "";
            foreach (ItemViewModel item in pivot.Items.Where(i => i.ToDelete == "Collapsed"))
            {
                list += item.Name + "\r\n";
            }

            // generate image for the front tile
            Tile tile = new Tile
            {
                title = { Text = pivot.Title },
                description = { Text = list },
            };

            SaveTile(tile, string.Format("/Shared/ShellContent/{0}.png", fileName));

            StandardTileData newTileData = new StandardTileData
            {
                Title = string.Empty,
                BackgroundImage = new Uri(string.Format("isostore:/Shared/ShellContent/{0}.png", fileName)),
            };

            if (update)
            {
                ShellTile tileToUpdate = ShellTile.ActiveTiles.First(t => t.NavigationUri == navUri);
                tileToUpdate.Update(newTileData);
                return;
            }
            // Create the tile and pin it to Start.
            ShellTile.Create(navUri, newTileData);     
        }

        // Создание картинки тайла
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