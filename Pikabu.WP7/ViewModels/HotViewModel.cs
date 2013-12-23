using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.IO;
using Pikabu.Helpers;
using Microsoft.Phone.Shell;

namespace Pikabu.ViewModels
{
    public delegate void DataLoadEventHandler();

    public class HotViewModel:INotifyPropertyChanged
    {
        /// <summary>
        /// Поле для загруженных историй
        /// </summary>
        private ObservableCollection<OnlineStory> stories;

        /// <summary>
        /// Событие начала загрузки
        /// </summary>
        public event DataLoadEventHandler DataLoadStarted;

        /// <summary>
        /// Событие окончания загрузки
        /// </summary>
        public event DataLoadEventHandler DataLoadCompleted;

        /// <summary>
        /// Событие срыва загрузки
        /// </summary>
        public event DataLoadEventHandler DataLoadFailed;

        /// <summary>
        /// Конструктор
        /// </summary>
        public HotViewModel()
        {
        }



        /// <summary>
        /// Свойство для доступа к загруженным новостям
        /// </summary>
        public ObservableCollection<OnlineStory> StoriesSource
        {
            get
            {
                if (stories == null)
                {
                    stories = new ObservableCollection<OnlineStory>();
                    LoadStories();
                }
                return stories;
            }
            private set
            {
                stories = value;
                this.RaisePropertyChanged("StoriesSource");
            }
        }


        /// <summary>
        /// Метод для загрузки новостей
        /// </summary>
        async public void LoadStories()
        {
            StoriesSource.Clear();
            if (DataLoadStarted != null)
            {
                DataLoadStarted();
            }
            string a = UriBuilder();
            try
            {
                string x = await OtherHelpers.LoadPageAsync(UriBuilder());
                XDocument xml = XDocument.Parse(x);
                ///Для оффлайн-кодинга
                //StreamResourceInfo streamxml = Application.GetResourceStream(new Uri("/Pikabu;component/data.xml", UriKind.Relative));
                //StreamReader strm = new StreamReader(streamxml.Stream);
                //string data = strm.ReadToEnd();
                //XDocument xml = XDocument.Parse(data);
                var result = (from n in xml.Descendants("item")
                              select new OnlineStory()
                              {
                                  ID = n.Element("id").Value,
                                  Pluses = n.Element("pluses").Value,
                                  Minuses = n.Element("minuses").Value,
                                  Rating = n.Element("rating").Value,
                                  Title = n.Element("title").Value,
                                  CommentsCount = n.Element("comment_count").Value,
                                  CommentsLink = n.Element("comment_link").Value,
                                  Image = n.Elements("image_link").Count() == 0 ? new Uri("/Images/noimage.png", UriKind.Relative) : new Uri(n.Element("image_link").Value),
                                  Author = n.Element("author").Value,
                                  Type = n.Elements("image_link").Count() == 0 ? StoryType.Text : StoryType.Image,
                                  Description = n.Elements("description").Count() == 0 ? "" : n.Element("description").Value,
                                  DescriptionText = n.Elements("image_link").Count() == 0 ? n.Element("description_text").Value : ""
                              });
                //foreach (var item in result)
                //{
                //    StoriesSource.Add(item);
                //}
                StoriesSource = new ObservableCollection<OnlineStory>(result);
                if (DataLoadCompleted != null)
                {
                    DataLoadCompleted();
                }
            }
            catch (Exception)
            {
                if (DataLoadFailed != null)
                {
                    DataLoadFailed();
                }
            }
        }

        


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string UriBuilder()
        {
            string uri = "http://pikabu.ru/generate_xml.php?amount=" + SettingsHelper.GetSettingFromIS<string>("listPickerPostCount", "100");
            if (SettingsHelper.GetSettingFromIS<bool>("checkBoxOnlyText", false))
            {
                uri += "&type=text";
            }
            return uri;
        }

        public void SaveLikeState()
        {
            PhoneApplicationService.Current.State["HotStories"] = this.stories;
        }
        public void LoadFromState()
        {
            this.stories = null;
            this.stories = PhoneApplicationService.Current.State["HotStories"] as ObservableCollection<OnlineStory>;
        }
    }
}
