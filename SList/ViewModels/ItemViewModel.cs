using System;
using System.ComponentModel;

namespace SList
{
    // Элементы списка
    public class ItemViewModel : INotifyPropertyChanged
    {
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