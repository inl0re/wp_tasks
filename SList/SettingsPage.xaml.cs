using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace SList
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AppSettings settings = (AppSettings)this.Resources["appSettings"];
        }
    }
}
