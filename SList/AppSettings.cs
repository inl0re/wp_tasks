using System;

using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace SList
{
    public class AppSettings
    {
        IsolatedStorageSettings isolatedStore;
        const string TileHeadSettingKeyName = "TileHeadSetting";

        const bool TileHeadSettingDefault = true;

        public AppSettings()
        {
            try
            {
                isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }

        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;
            // If the key exists
            if (isolatedStore.Contains(Key))
            {
                // If the value has changed
                if (isolatedStore[Key] != value)
                {
                    // Store the new value
                    isolatedStore[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (isolatedStore.Contains(Key))
            {
                value = (valueType)isolatedStore[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            isolatedStore.Save();
        }

        //Checkbox
        public bool TileHeadSetting
        {
            get
            {
                return GetValueOrDefault<bool>(TileHeadSettingKeyName, TileHeadSettingDefault);
            }
            set
            {
                AddOrUpdateValue(TileHeadSettingKeyName, value);
                Save();
            }
        }
    }
}