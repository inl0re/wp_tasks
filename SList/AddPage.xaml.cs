using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SList
{
    public partial class AddPage : PhoneApplicationPage
    {
        public AddPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(AddPage_Loaded);
        }

        private void AddPage_Loaded(object sender, RoutedEventArgs e)
        {
            NameNewList.Focus();
        }
        private void AddTile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameNewList.Text))
            {
                MessageBox.Show("Не введено имя списка");
            }
            else
            {
                App.ViewModel.AddPivot(NameNewList.Text);
                Pivots pivot = App.ViewModel.PivotsList.First(p => p.Title == NameNewList.Text);
                NavigationService.Navigate(new Uri("/MainPage.xaml?title=" + NameNewList.Text, UriKind.Relative));
            }
        }
    }
}