using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Pikabu.Models;
using Microsoft.Phone.Shell;
using System.Collections;

namespace Pikabu
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Индикация страницы
        /// </summary>
        ProgressIndicator progressIndicator;

        bool hotIsLoaded;

        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            progressIndicator = new ProgressIndicator() { IsVisible = false, IsIndeterminate = true };
            SystemTray.SetProgressIndicator(this, progressIndicator);
            App.HotStoryVM.DataLoadStarted += () => 
            {
                LLSHot.Visibility = System.Windows.Visibility.Visible;
                errorTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                progressIndicator.Text = "Загрузка..."; 
                progressIndicator.IsVisible = true;
            };
            App.HotStoryVM.DataLoadCompleted += () =>
            {
                LLSHot.Visibility = System.Windows.Visibility.Visible;
                errorTextBlock.Visibility = System.Windows.Visibility.Collapsed;
                progressIndicator.IsVisible = false;
                BestStoryTab.DataContext = App.BestHotVM;
                hotIsLoaded = true;
            };
            App.HotStoryVM.DataLoadFailed += () => 
            {
                LLSHot.Visibility = System.Windows.Visibility.Collapsed;
                errorTextBlock.Visibility = System.Windows.Visibility.Visible;
                progressIndicator.IsVisible = false; 
            };
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            HotStoryTab.DataContext = App.HotStoryVM;
        }

        private void HotStoryLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector lls = sender as LongListSelector;
            int selectedIndex = App.HotStoryVM.StoriesSource.IndexOf(lls.SelectedItem as OnlineStory);
            ((LongListSelector)sender).SelectionChanged -= HotStoryLLS_SelectionChanged;
            ((LongListSelector)sender).SelectedItem = -1;
            ((LongListSelector)sender).SelectionChanged += HotStoryLLS_SelectionChanged;
            NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=hot&NavigationType=main&SelectedIndex=" + selectedIndex, UriKind.Relative));
        }

        private void LLSBestHot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector lls = sender as LongListSelector;
            int selectedIndex = App.BestHotVM.BestStoriesSource.IndexOf(lls.SelectedItem as OnlineStory);
            ((LongListSelector)sender).SelectionChanged -= LLSBestHot_SelectionChanged;
            ((LongListSelector)sender).SelectedItem = -1;
            ((LongListSelector)sender).SelectionChanged += LLSBestHot_SelectionChanged;
            NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=besthot&NavigationType=main&SelectedIndex=" + selectedIndex, UriKind.Relative));
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            App.HotStoryVM.LoadStories();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void Favorites_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FavoritesPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}