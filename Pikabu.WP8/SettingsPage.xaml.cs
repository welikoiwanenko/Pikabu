using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pikabu.Helpers;
using Coding4Fun.Toolkit.Controls;

namespace Pikabu
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CacheHelper.FavoritesDeletionAllCompleted += CacheHelper_FavoritesDeletionAllCompleted;
            string isNew = string.Empty;
            if (e.NavigationMode != NavigationMode.Back)
            {
                checkBoxOnlyText.IsChecked = SettingsHelper.GetSettingFromIS<bool>("checkBoxOnlyText", false);
                listPickerPostCount.SelectedItem = SettingsHelper.GetSettingFromIS<string>("listPickerPostCount", "100");
                listPickerSortType.SelectedItem = SettingsHelper.GetSettingFromIS<string>("listPickerSortType", "время");
            }
            base.OnNavigatedTo(e);
        }

        void CacheHelper_FavoritesDeletionAllCompleted()
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Title = "Pikabu";
            toast.Message = "Избранное очищено";
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CacheHelper.FavoritesDeletionAllCompleted -= CacheHelper_FavoritesDeletionAllCompleted;
            base.OnNavigatedFrom(e);
            if (!e.Uri.OriginalString.Contains("ListPickerPage.xaml"))
            {
                SettingsHelper.SetSettingInIS<bool>(checkBoxOnlyText.IsChecked, "checkBoxOnlyText");
                SettingsHelper.SetSettingInIS<string>(listPickerPostCount.SelectedItem, "listPickerPostCount");
                SettingsHelper.SetSettingInIS<string>(listPickerSortType.SelectedItem, "listPickerSortType");
            }
        }

        private void cacheCrearerButton_Click(object sender, RoutedEventArgs e)
        {
            CacheHelper.DeleteAllOfflinePosts(OfflinePostType.Favorites);
        }

        

        
    }
}