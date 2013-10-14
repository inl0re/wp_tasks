﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SList
{
    // Элементы списка
    public class ItemViewModel : INotifyPropertyChanged
    {
        /*
        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private List<string> _data;

        public List<string> Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    NotifyPropertyChanged("Data");
                }
            }
        }
         */

        private string _name;
        /// <summary>
        /// Пример свойства ViewModel; это свойство используется в представлении для отображения его значения с помощью привязки.
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _toDelete;

        public string ToDelete
        {
            get
            {
                return _toDelete;
            }
            set
            {
                if (value != _toDelete)
                {
                    _toDelete = value;
                    NotifyPropertyChanged("ToDelete");
                }
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