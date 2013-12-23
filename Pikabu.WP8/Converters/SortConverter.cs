using Pikabu.Helpers;
using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Pikabu.Converters
{
    public class SortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string a = SettingsHelper.GetSettingFromIS<string>("listPickerSortType", "время");
            if (SettingsHelper.GetSettingFromIS<string>("listPickerSortType", "время") == "имя")
            {
                var list = ((ObservableCollection<OnlineStory>)value).OrderBy(o => o.Title).ToList();
                return list;
            }
            else if (SettingsHelper.GetSettingFromIS<string>("listPickerSortType", "время") == "рейтинг")
            {
                var list = ((ObservableCollection<OnlineStory>)value).OrderBy(o => System.Convert.ToInt32(o.Rating)).Reverse().ToList();
                return list;
            }
            else
            {
                var list = ((ObservableCollection<OnlineStory>)value).OrderBy(o => System.Convert.ToInt32(o.ID)).Reverse().ToList(); ;
                return list;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
