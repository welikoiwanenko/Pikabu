using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Pikabu.Helpers;
using Microsoft.Phone.Shell;

namespace Pikabu.ViewModels
{
    public class BestHotViewModel : INotifyPropertyChanged
    {
        public BestHotViewModel()
        {
            
        }

        private ObservableCollection<OnlineStory> bestStoriesSource;

        public ObservableCollection<OnlineStory> BestStoriesSource
        {
            get 
            {
                ObservableCollection<OnlineStory> hot = App.HotStoryVM.StoriesSource;
                bestStoriesSource = hot.GetBest20();
                return bestStoriesSource; 
            }
        }

        public void SaveLikeState()
        {
            PhoneApplicationService.Current.State["BetsHotStories"] = this.bestStoriesSource;
        }
        public void LoadFromState()
        {
            this.bestStoriesSource = null;
            this.bestStoriesSource = PhoneApplicationService.Current.State["BetsHotStories"] as ObservableCollection<OnlineStory>;
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
    }
}
