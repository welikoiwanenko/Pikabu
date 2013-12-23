using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pikabu.Helpers
{
    public static class SettingsHelper
    {
        public static void SetSettingInIS<T>(object setting, string name)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings appSettings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains(name))
            {
                appSettings[name] = setting;
            }
            else
            {
                appSettings.Add(name, setting);
            }
        }
        public static T GetSettingFromIS<T>(string name, T defaultValue)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings appSettings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains(name))
            {
                try
                {
                    return (T)appSettings[name];
                }
                catch (InvalidCastException)
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
