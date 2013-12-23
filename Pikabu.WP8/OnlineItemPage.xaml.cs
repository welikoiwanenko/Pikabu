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
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media.Imaging;

namespace Pikabu
{
    public partial class OnlineItemPage : PhoneApplicationPage, IItemPage
    {
        ToastPrompt toast;
        private string NavigationType;
        private string SelectedIndex;
        private OnlineStory currentStory;
        private string ListType;

        public OnlineItemPage()
        {
            InitializeComponent();
            toast = new ToastPrompt();
        }

        /// <summary>
        /// Выполняется при открытии страницы
        /// Получаемые параметры - тип перехода navType(main - из главной / post - листанием),
        /// тип списка, из которого произошла навигация (hot - если из горчего, bestрot - если из лучшего горчего) и selectedindex
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CacheHelper.FavoritesCreationCompleted += CacheHelper_FavoritesCreationCompleted;
            CacheHelper.FavoritesDeletionCompleted += CacheHelper_FavoritesDeletionCompleted;
            if (e.IsNavigationInitiator) //если активация страницы произошла из самого приложения
            {
                NavigationContext.QueryString.TryGetValue("NavigationType", out NavigationType);
                NavigationContext.QueryString.TryGetValue("SelectedIndex", out SelectedIndex);
                NavigationContext.QueryString.TryGetValue("ListType", out ListType);
                if (NavigationType == "post"&&e.NavigationMode != NavigationMode.Back)
                {
                    NavigationService.RemoveBackEntry();
                }
                if (ListType == "hot")
                {
                    currentStory = App.HotStoryVM.StoriesSource[Convert.ToInt32(SelectedIndex)];
                }
                else
                {
                    currentStory = App.BestHotVM.BestStoriesSource[Convert.ToInt32(SelectedIndex)];
                }
            }
            else //если из меню многозадачности
            {
                App.HotStoryVM.LoadFromState();
                App.BestHotVM.LoadFromState();
                NavigationType = PhoneApplicationService.Current.State["NavigationType"].ToString();
                SelectedIndex = PhoneApplicationService.Current.State["SelectedIndex"].ToString();
                currentStory = PhoneApplicationService.Current.State["CurrentStory"] as OnlineStory;
                ListType = PhoneApplicationService.Current.State["ListType"].ToString();
            }
            SetBrowserContent();
            title.Text = currentStory.Title;
            AppBarGoButtonEnablingDefinition(Convert.ToInt32(SelectedIndex));
            AppBarFavoritesButtonSetIcon();
        }

        /// <summary>
        /// Отображение поста на странице
        /// </summary>
        private void SetBrowserContent()
        {
            if (currentStory == null)
            {
                //((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;
                throw new NullReferenceException("Пост удалён либо не существует.");
            }
            else if (currentStory.Type == StoryType.Image)
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

        /// <summary>
        /// Выполняется перед деактивацией страницы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            
            CacheHelper.FavoritesCreationCompleted -= CacheHelper_FavoritesCreationCompleted;
            CacheHelper.FavoritesDeletionCompleted -= CacheHelper_FavoritesDeletionCompleted;
            if (!e.IsNavigationInitiator)
            {
                PhoneApplicationService.Current.State["NavigationType"] = NavigationType;
                PhoneApplicationService.Current.State["SelectedIndex"] = SelectedIndex;
                PhoneApplicationService.Current.State["CurrentStory"] = currentStory;
                PhoneApplicationService.Current.State["ListType"] = ListType;
                App.HotStoryVM.SaveLikeState();
                App.BestHotVM.SaveLikeState();
            }
        }

        /// <summary>
        /// Метод для определения доступности кнопок и пунктов меню AppBar
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void AppBarGoButtonEnablingDefinition(int selectedIndex)
        {
            if (ListType == "hot")
            {
                if (selectedIndex == 0 & selectedIndex == (App.HotStoryVM.StoriesSource.Count - 1))
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                }
                else if (selectedIndex == 0)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                }
                else if (selectedIndex == (App.HotStoryVM.StoriesSource.Count - 1))
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                }
            }
            else
            {
                if (selectedIndex == 0 & selectedIndex == (App.BestHotVM.BestStoriesSource.Count - 1))
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                }
                if (selectedIndex == 0)
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
                }
                else if (selectedIndex == (App.BestHotVM.BestStoriesSource.Count - 1))
                {
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Установка иконки клавиши действия над изранным(add to или delete from)
        /// </summary>
        public void AppBarFavoritesButtonSetIcon()
        {
            var favStoriesID = CacheHelper.GetOfflinePostsID(OfflinePostType.Favorites);
            if (favStoriesID.Contains(currentStory.ID))
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IconUri = new Uri("Assets/AppBar/appbar.favs.delfrom.rest.png", UriKind.Relative);
            }
            else
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IconUri = new Uri("Assets/AppBar/appbar.favs.addto.rest.png", UriKind.Relative);
            }
        }

        /// <summary>
        /// Нажатие на клавишу "Назад"
        /// </summary>
        private void PostBack_Click(object sender, EventArgs e)
        {
            this.GoBack();
        }

        /// <summary>
        /// Метод для перехода на предыдущую страницу
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
                if (ListType == "hot")
                {
                    NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=hot&NavigationType=post&SelectedIndex=" + indexPrevStory, UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=besthot&NavigationType=post&SelectedIndex=" + indexPrevStory, UriKind.Relative));
                }
            }
            catch (Exception)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
            }
        }

        /// <summary>
        /// Нажатие на клавишу "Вперёд"
        /// </summary>
        private void PostNext_Click(object sender, EventArgs e)
        {
            GoForward();
        }

        /// <summary>
        /// Методя для перехода на следующую страницу
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
                if (ListType == "hot")
                {
                    NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=hot&NavigationType=post&SelectedIndex=" + indexNextStory, UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/OnlineItemPage.xaml?ListType=besthot&NavigationType=post&SelectedIndex=" + indexNextStory, UriKind.Relative));
                }
            }
            catch (Exception)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
        }

        /// <summary>
        /// Действие при нажатии клавиши "добавить в избранное"/"удалить из избранного"
        /// </summary>
        private void Favorite_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton button = (ApplicationBarIconButton)sender;
            if (button.IconUri.ToString().Contains("addto"))
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
            else
            {
                try
                {
                    CacheHelper.DeleteOfflinePost(currentStory.ID, OfflinePostType.Favorites);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Возникла ошибка:(");
                    BugSense.BugSenseHandler.Instance.SendExceptionAsync(ex, "type", "delfromfav");
                }
            }
        }

        private void SaveImage_Click(object sender, EventArgs e)
        {
            SaveImage();
        }

        /// <summary>
        /// Сохранение картинки из поста
        /// </summary>
        public void SaveImage()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += (s, ee) =>
            {
                if (ee.Error == null)
                {
                    MediaLibrary library = new MediaLibrary();
                    library.SavePicture(currentStory.Title, ee.Result);
                    toast.Title = "Pikabu";
                    toast.Message = "Изображение сохранено";
                    toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
                    toast.Show();
                }
            };
            client.OpenReadAsync(((OnlineStory)currentStory).Image);
        }

        private void OpenPostInBrowser_Click(object sender, EventArgs e)
        {
            OpenInBrowser(new Uri(currentStory.CommentsLink));
        }

        /// <summary>
        /// Открытие поста в браузере
        /// </summary>
        /// <param name="postUri"></param>
        public void OpenInBrowser(Uri postUri)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = postUri;
            webBrowserTask.Show();
        }

        /// <summary>
        /// Завершение добавления поста в избранное
        /// </summary>
        void CacheHelper_FavoritesCreationCompleted()
        {
            ApplicationBarIconButton a = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            ApplicationBar.Buttons.RemoveAt(1);
            a.IconUri = new Uri("Assets/AppBar/appbar.favs.delfrom.rest.png", UriKind.Relative);
            ApplicationBar.Buttons.Insert(1, a);
            toast.Title = "Pikabu";
            toast.Message = "Добавлено в избранное";
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

        /// <summary>
        /// Завершение удаления поста из избранного
        /// </summary>
        void CacheHelper_FavoritesDeletionCompleted()
        {
            ApplicationBarIconButton a = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            ApplicationBar.Buttons.RemoveAt(1);
            a.IconUri = new Uri("Assets/AppBar/appbar.favs.addto.rest.png", UriKind.Relative);
            ApplicationBar.Buttons.Insert(1, a);
            toast.Title = "Pikabu";
            toast.Message = "Удалено из избранного";
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;
            toast.Show();
        }

        private void OpenComment_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CommentsPage.xaml?PostID=" + currentStory.ID, UriKind.Relative));
        }
    }
}