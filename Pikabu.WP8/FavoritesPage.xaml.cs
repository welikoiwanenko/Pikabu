using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pikabu.Models;
using Pikabu.Helpers;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;

namespace Pikabu
{
    public partial class FavoritesPage : PhoneApplicationPage
    {
        ToastPrompt toast;
        public FavoritesPage()
        {
            InitializeComponent();
            toast = new ToastPrompt();
            CacheHelper.FavoritesDeletionCompleted += CacheHelper_FavoritesDeletionCompleted;
        }

        void CacheHelper_FavoritesDeletionCompleted()
        {
            toast.Title = "Pikabu";
            toast.Message = "Удалено из избранного";
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

        

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.FavoritesVM;
        }

        private void FavoritesStoryLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id = ((OfflineStory)(((LongListSelector)sender).SelectedItem)).ID;
            int selectedIndex = -1;
            var a = CacheHelper.GetOfflinePostsID(OfflinePostType.Favorites);
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] == id)
                {
                    selectedIndex = i;
                    break;
                }
            }
            ((LongListSelector)sender).SelectionChanged -= FavoritesStoryLLS_SelectionChanged;
            ((LongListSelector)sender).SelectedItem = -1;
            ((LongListSelector)sender).SelectionChanged += FavoritesStoryLLS_SelectionChanged;
            NavigationService.Navigate(new Uri("/OfflineItemPage.xaml?NavigationType=main&SelectedIndex=" + selectedIndex, UriKind.Relative));

        }

        private void DeleteFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            OfflineStory selectedAlbum = (sender as MenuItem).DataContext as OfflineStory;
            CacheHelper.DeleteOfflinePost(selectedAlbum.ID, OfflinePostType.Favorites);
            this.DataContext = App.FavoritesVM;
        }
    }
}