using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Pikabu.Models
{
    public class OnlineStory:Story
    {
        private string pluses;
        public string Pluses
        {
            get
            {
                return pluses;
            }
            set
            {
                if (this.pluses != value)
                {
                    this.pluses = value;
                    this.RaisePropertyChanged("Pluses");
                }
            }
        }
        private string commentsLink;
        public string CommentsLink
        {
            get
            {
                return commentsLink;
            }
            set
            {
                if (this.commentsLink != value)
                {
                    this.commentsLink = value;
                    this.RaisePropertyChanged("CommentsLink");
                }
            }
        }
        private string minuses;
        public string Minuses
        {
            get
            {
                return minuses;
            }
            set
            {
                if (this.minuses != value)
                {
                    this.minuses = value;
                    this.RaisePropertyChanged("Minuses");
                }
            }
        }

        private string commentsCount;
        public string CommentsCount
        {
            get
            {
                return commentsCount;
            }
            set
            {
                if (this.commentsCount != value)
                {
                    this.commentsCount = value;
                    this.RaisePropertyChanged("CommentsCount");
                }
            }
        }

        private string rating;
        public string Rating
        {
            get
            {
                return rating;
            }
            set
            {
                if (this.rating != value)
                {
                    this.rating = value;
                    this.RaisePropertyChanged("Rating");
                }
            }
        }

        private Uri image;
        public Uri Image
        {
            get
            {
                return image;
            }
            set
            {
                if (this.image != value)
                {
                    this.image = value;
                    this.RaisePropertyChanged("Image");
                }
            }
        }

        private string author;
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                if (this.author != value)
                {
                    this.author = value;
                    this.RaisePropertyChanged("Author");
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

        public string CommentLink { get; set; }
    }
    public enum StoryType
    {
        Image, Text,
        Offline
    }
}
