﻿using System;
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
using System.Diagnostics;

namespace SList
{
    public partial class StartPage : PhoneApplicationPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (fileStorage.DirectoryExists("Data") && fileStorage.GetFileNames("Data\\*").Length > 0)
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnNavigatedTo(e);    
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

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)); 
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)); 
        }

    }
}