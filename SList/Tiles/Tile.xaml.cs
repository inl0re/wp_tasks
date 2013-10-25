using Microsoft.Phone.Shell;
using ProTile.Lib;
using SList;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProTile
{
	public partial class Tile
	{
		public Tile()
		{
			InitializeComponent();
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("TileHeadSetting"))
            {
                title.Visibility = ((bool)appSettings["TileHeadSetting"] == true) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                appSettings["TileHeadSetting"] = true;
		}
        public void AddTile(Pivots pivot, bool update) // Создаём тайл для списка
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

            SaveTile(tile, string.Format("/Shared/ShellContent/{0}.png", fileName)); // Сохраняем файл бэка тайла

            StandardTileData newTileData = new StandardTileData // Создаём данные, которые будут исп-ны при создании тайла
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

            ShellTile.Create(navUri, newTileData); // Создаём и закрепляем тайл
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
	}
}
