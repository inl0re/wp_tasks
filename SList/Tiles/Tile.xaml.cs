using System.IO.IsolatedStorage;
using System.Windows;

namespace ProTile
{
	public partial class Tile
	{
		public Tile()
		{
			// Required to initialize variables
			InitializeComponent();
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("TileHeadSetting"))
            {
                title.Visibility = ((bool)appSettings["TileHeadSetting"] == true) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                appSettings["TileHeadSetting"] = true;
		}
	}
}
