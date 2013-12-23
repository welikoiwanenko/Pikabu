using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pikabu.Helpers;
using System.Windows.Input;

namespace Pikabu.ViewModels
{
    public class FavoritesViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<OfflineStory> stories;
        private ICommand deleteCommand;
        public FavoritesViewModel()
        {
            this.deleteCommand = new DelegateCommand(this.DeleteDataAction);
        }
        public ICommand DeleteCommand
        {
            get
            {
                return this.deleteCommand;
            }
        }
        private void DeleteDataAction(object obj)
        {
            //CacheHelper.DeleteOfflinePost(
        }

        public ObservableCollection<OfflineStory> StoriesSource
        {
            get
            {
                if (stories == null)
                {
                    stories = new ObservableCollection<OfflineStory>();
                    SetPosts();
                }
                return stories;
            }
        }

        public void SetPosts()
        {
            var a = Helpers.CacheHelper.OpenOfflinePosts(Helpers.OfflinePostType.Favorites).ToList<OfflineStory>();
            foreach (var item in a)
            {
                StoriesSource.Add(item);
            }
        }
        //private IList<OfflineStory> GetFavoriteStories()
        //{
        //    return await Helpers.CacheHelper.OpenOfflinePosts(Helpers.OfflinePostType.Favorites);
        //}
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
