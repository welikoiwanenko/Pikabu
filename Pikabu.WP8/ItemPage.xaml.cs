using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pikabu.ViewModels;
using Pikabu.Models;
using Microsoft.Xna.Framework.Media;
using Pikabu.Helpers;
using Microsoft.Phone.Tasks;

namespace Pikabu
{
    public partial class ItemPage : PhoneApplicationPage
    {
        /// <summary>
        /// История для отображения на текущей странице
        /// </summary>
        Story currentStory;

        /// <summary>
        /// Индикация страницы
        /// </summary>
        ProgressIndicator progressIndicator;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ItemPage()
        {
            InitializeComponent();
        }

        

        

        void CacheHelper_FavoritesCreationCompleted()
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
        }

        /// <summary>
        /// Событие перехода на страницу
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                
                #region Инициализация ProgressIndicator
                progressIndicator = new ProgressIndicator { IsVisible = false };
                progressIndicator.IsIndeterminate = true;
                SystemTray.SetProgressIndicator(this, progressIndicator);
                #endregion
                CacheHelper.FavoritesCreationCompleted += CacheHelper_FavoritesCreationCompleted;
            }
            catch
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
            }
            //if (e.NavigationMode == NavigationMode.New)
            {
                //base.OnNavigatedTo(e);

                progressIndicator.Text = "Загрузка...";
                progressIndicator.IsVisible = true;

                /// Определяем, каким образом осуществлялся переход к странице. Если листанием постов - удаляем предыдущую запись в журнале переходов.
                #region
                try
                {
                string isFlickNavigate = string.Empty;
                if (NavigationContext.QueryString.TryGetValue("isFlickNavigate", out isFlickNavigate))
                    NavigationService.RemoveBackEntry();
                }
                catch (Exception)
                {

                }
                #endregion

                /// Находим пост с полученным ID и отображаем его в WebBrowser
                #region
                string id = string.Empty;
                string type = string.Empty;
                try
                {
                    if (NavigationContext.QueryString.TryGetValue("id", out id) && NavigationContext.QueryString.TryGetValue("type", out type))
                    {
                        if (type == "online")
                        {
                            foreach (var item in App.HotStoryVM.StoriesSource)
                            {
                                if (item.ID == id)
                                {
                                    currentStory = item;
                                    SetEnabledForAppBar(currentStory);
                                    title.Text += item.Title;
                                    break;
                                }
                            }
                            //webBrowser.Navigated += (s, ee) => { progressIndicator.IsVisible = false; };
                            webBrowser.NavigationFailed += webBrowser_NavigationFailed;
                            if (currentStory == null)
                            {
                                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;
                                throw new NullReferenceException("Пост удалён либо не существует.");
                            }
                            if (currentStory.Type == StoryType.Image)
                            {
                                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = true;
                                webBrowser.NavigateToString("<html><body></br> <image style='width:100%;margin:12' src=\'" + ((OnlineStory)currentStory).Image.ToString() + "\'></img> </br> <p style='margin:12'>" + OtherHelpers.ConvertExtendedASCII(currentStory.Description) + "</p></body></html>");
                            }
                            else if (currentStory.Type == StoryType.Text)
                            {
                                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;
                                webBrowser.NavigateToString("<html><body> </br> <p style='margin:12'>" + OtherHelpers.ConvertExtendedASCII(currentStory.DescriptionText) + "</p></body></html>");
                            }
                        }
                        else if (type == "offline")
                        {
                            foreach (var item in App.FavoritesVM.StoriesSource)
                            {
                                if (item.ID == id)
                                {
                                    currentStory = item;
                                    SetEnabledForAppBar(currentStory);
                                    title.Text += item.Title;
                                    break;
                                }
                            }
                            webBrowser.Base = "Favorites\\" + id;
                            webBrowser.Navigate(new Uri(id + ".html", UriKind.Relative));
                            title.Text = CacheHelper.GetOfflinePostTitle(id, OfflinePostType.Favorites);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Возникла ошибка:(");
                    BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex);
                }
                finally
                {
                    progressIndicator.IsVisible = false;
                }
                #endregion
            }
            //else
            //{
            //    SetEnabledForAppBar(currentStory);
            //}
        }

        void webBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show("Ошибка загрузки! Пост удалён либо не существует.");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            webBrowser.NavigationFailed -= webBrowser_NavigationFailed;
            CacheHelper.FavoritesCreationCompleted -= CacheHelper_FavoritesCreationCompleted;
        }
        /// <summary>
        /// Определение доступности кнопок "Вперёд" и "Назад" исходя из номера поста 
        /// </summary>
        /// <param name="item"></param>
        private void SetEnabledForAppBar(Story story)
        {
            int i = 0;
            string type = "";
            try
            {
                //1
                i++;
                if (NavigationContext.QueryString.TryGetValue("type", out type))
                {
                    //2
                    i++;
                    if (type == "online")
                    {
                        //3
                        i++;
                        if (App.HotStoryVM.StoriesSource.IndexOf((OnlineStory)story) == 0)
                        {
                            //4
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                            //5
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                        }
                        else if (App.HotStoryVM.StoriesSource.IndexOf((OnlineStory)story) == App.HotStoryVM.StoriesSource.Count - 1)
                        {
                            //6
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                            //7
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                        }
                        else
                        {
                            //8
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                            //9
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                        }

                        //10
                        i++;
                        ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;


                        //11
                        i++;
                        if (CacheHelper.GetOfflinePostsID(OfflinePostType.Favorites).Contains(story.ID))
                        {
                            //12
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                        }
                        else
                        {
                            //13
                            i++;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                        }
                        //14
                        i++;
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = true;


                    }
                    else if (type == "offline")
                    {
                        int index = CacheHelper.GetOfflinePostsID(OfflinePostType.Favorites).IndexOf(story.ID);
                        if (index == 0)
                        {
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                        }
                        else if (index == App.FavoritesVM.StoriesSource.Count - 1)
                        {
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                        }
                        else if (App.FavoritesVM.StoriesSource.Count == 1)
                        {
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                        }
                        else
                        {
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                        }
                        ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = false;


                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка:(");
                //BugSense.Models.CrashExtraData a = new BugSense.Models.CrashExtraData() { Key = "i", Value = i.ToString() };
                //BugSense.BugSenseHandler.Instance.AddCrashExtraData(a);
                BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex, "i", i.ToString());
            }
        }




        /// <summary>
        /// Переход по постам назад
        /// </summary>
        private void GoBack()
        {
            try
            {
                //Анимация ухода картинки вправо
                NavigationOutTransition navigationOutTransition = new NavigationOutTransition()
                {
                    Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                    Forward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut }
                };
                TransitionService.SetNavigationOutTransition(this, navigationOutTransition);

                string type;
                if (NavigationContext.QueryString.TryGetValue("type", out type))
                {
                    if (type == "online")
                    {
                        int indexCurrentStory = App.HotStoryVM.StoriesSource.IndexOf((OnlineStory)this.currentStory);
                        try
                        {
                            string idPrevStory = App.HotStoryVM.StoriesSource[indexCurrentStory - 1].ID;
                            NavigationService.Navigate(new Uri("/ItemPage.xaml?id=" + idPrevStory + "&isFlickNavigate=true&type=online", UriKind.Relative));
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            progressIndicator.Text = "Вы достигли начала";
                            progressIndicator.IsVisible = true;
                        }
                    }
                    else if (type == "offline")
                    {
                        string selectedIndex;
                        if (NavigationContext.QueryString.TryGetValue("selectedIndex", out selectedIndex))
                        {
                            try
                            {
                                string idPrevStory = App.FavoritesVM.StoriesSource[Convert.ToInt32(selectedIndex) - 1].ID;
                                NavigationService.Navigate(new Uri("/ItemPage.xaml?id=" + idPrevStory + "&isFlickNavigate=true&type=offline&selectedIndex=" + (Convert.ToInt32(selectedIndex) - 1).ToString(), UriKind.Relative));
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                progressIndicator.Text = "Вы достигли начала";
                                progressIndicator.IsVisible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка:(");
                BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex, "type", "gobackonline");
            }
        }

        /// <summary>
        /// Переход по постам вперёд
        /// </summary>
        private void GoForward()
        {
            try
            {
                
                //Анимация ухода картинки влево
                NavigationOutTransition navigationOutTransition = new NavigationOutTransition()
                {
                    Backward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut },
                    Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
                };
                TransitionService.SetNavigationOutTransition(this, navigationOutTransition);

                string type;
                if (NavigationContext.QueryString.TryGetValue("type", out type))
                {

                    if (type == "online")
                    {
                        int indexCurrentStory = App.HotStoryVM.StoriesSource.IndexOf((OnlineStory)this.currentStory);

                        try
                        {
                            string idNextStory = App.HotStoryVM.StoriesSource[indexCurrentStory + 1].ID;

                            NavigationService.Navigate(new Uri("/ItemPage.xaml?id=" + idNextStory + "&isFlickNavigate=true&type=online", UriKind.Relative));
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            progressIndicator.Text = "Вы достигли начала";
                            progressIndicator.IsVisible = true;
                        }
                    }
                    else if (type == "offline")
                    {
                        string selectedIndex;
                        if (NavigationContext.QueryString.TryGetValue("selectedIndex", out selectedIndex))
                        {
                            try
                            {
                                string idNextStory = App.FavoritesVM.StoriesSource[Convert.ToInt32(selectedIndex) + 1].ID;
                                NavigationService.Navigate(new Uri("/ItemPage.xaml?id=" + idNextStory + "&isFlickNavigate=true&type=offline&selectedIndex=" + (Convert.ToInt32(selectedIndex) + 1).ToString(), UriKind.Relative));
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                progressIndicator.Text = "Вы достигли конца";
                                progressIndicator.IsVisible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка:(");
                BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex, "type", "goforwardonline");
            }
        }

        /// <summary>
        /// Нажатие на кнопку "Назад"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// Нажатие на кнопку "Вперёд"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostNext_Click(object sender, EventArgs e)
        {
            GoForward();
        }



        /// <summary>
        /// Сохранение рисунка в галерею
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImage_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += (s, ee) =>
            {
                if (ee.Error == null)
                {
                    MediaLibrary library = new MediaLibrary();
                    library.SavePicture(currentStory.Title, ee.Result);
                    progressIndicator.IsVisible = false;
                }
            };
            client.OpenReadAsync(((OnlineStory)currentStory).Image);
            progressIndicator.Text = "Сохранение...";
            progressIndicator.IsVisible = true;
        }

        private void Favorite_Click(object sender, EventArgs e)
        {
            try
            {
                CacheHelper.CreateOfflinePost((OnlineStory)currentStory, OfflinePostType.Favorites);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка:(");
                BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex, "type", "addtofav");
            }
        }

        private void OpenPostInBrowser_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(((OnlineStory)currentStory).CommentsLink);

            webBrowserTask.Show();
        }
    }
}