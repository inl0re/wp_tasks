using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;

namespace SList
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
            var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (fileStorage.GetFileNames().Length > 0)
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void AddIconButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputBox.Text) && InputBox.Text.Length < 28)
            {
                App.ViewModel.NewPivot(InputBox.Text);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}