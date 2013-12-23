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

namespace Pikabu
{
    public partial class OfflineItemPage : PhoneApplicationPage, IItemPage
    {
        private OfflineStory currentStory;
        private string NavigationType;
        private string SelectedIndex;


        public OfflineItemPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator) //если активация страницы произошла из самого приложения
            {
                NavigationContext.QueryString.TryGetValue("NavigationType", out NavigationType);
                NavigationContext.QueryString.TryGetValue("SelectedIndex", out SelectedIndex);
                if (NavigationType == "post")
                {
                    NavigationService.RemoveBackEntry();
                }
                currentStory = App.FavoritesVM.StoriesSource[Convert.ToInt32(SelectedIndex)];
                SetBrowserContent();
                title.Text = currentStory.Title;
            }
            else //если из меню многозадачности
            {
                NavigationType = PhoneApplicationService.Current.State["NavigationType"].ToString();
                SelectedIndex = PhoneApplicationService.Current.State["SelectedIndex"].ToString();
            }
            AppBarGoButtonEnablingDefinition(Convert.ToInt32(SelectedIndex));
        }

        private void SetBrowserContent()
        {
            webBrowser.Base = "Favorites\\" + currentStory.ID;
            webBrowser.Navigate(new Uri(currentStory.ID + ".html", UriKind.Relative));
            title.Text = CacheHelper.GetOfflinePostTitle(currentStory.ID, OfflinePostType.Favorites);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                PhoneApplicationService.Current.State["NavigationType"] = NavigationType;
                PhoneApplicationService.Current.State["SelectedIndex"] = SelectedIndex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void AppBarGoButtonEnablingDefinition(int selectedIndex)
        {
            if (selectedIndex == 0 & selectedIndex == (App.FavoritesVM.StoriesSource.Count - 1))
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            }
            else if (selectedIndex == 0)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            }
            else if (selectedIndex == (App.FavoritesVM.StoriesSource.Count - 1))
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }
        /// <summary>
        /// 
        /// </summary>
        public void GoBack()
        {
            NavigationOutTransition navigationOutTransition = new NavigationOutTransition()
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut }
            };
            TransitionService.SetNavigationOutTransition(this, navigationOutTransition);
            try
            {
                string indexPrevStory = (Convert.ToInt32(SelectedIndex) - 1).ToString();
                NavigationService.Navigate(new Uri("/OfflineItemPage.xaml?NavigationType=post&SelectedIndex=" + indexPrevStory, UriKind.Relative));
            }
            catch (Exception)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostNext_Click(object sender, EventArgs e)
        {
            GoForward();
        }
        /// <summary>
        /// 
        /// </summary>
        public void GoForward()
        {
            NavigationOutTransition navigationOutTransition = new NavigationOutTransition()
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
            };
            TransitionService.SetNavigationOutTransition(this, navigationOutTransition);

            try
            {
                string indexNextStory = (Convert.ToInt32(SelectedIndex) + 1).ToString();
                NavigationService.Navigate(new Uri("/OfflineItemPage.xaml?NavigationType=post&SelectedIndex=" + indexNextStory, UriKind.Relative));
            }
            catch (Exception)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
        }
    }
}