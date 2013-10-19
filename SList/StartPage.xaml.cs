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
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (fileStorage.GetFileNames().Length > 0)
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void AddIconButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text))
                return;
            if (InputBox.Text.Length > 16)
            {
                MessageBox.Show("Название списка не может быть длинее 16 символов");
                return;
            }
            App.ViewModel.NewPivot(InputBox.Text);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));         
        }

    }
}