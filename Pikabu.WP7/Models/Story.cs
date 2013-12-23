using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pikabu.Models
{
    public class Story : INotifyPropertyChanged
    {
        private string id;
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.RaisePropertyChanged("Title");
                }
            }
        }


        private StoryType type;
        public StoryType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }

        private string descriptionText;
        public string DescriptionText
        {
            get
            {
                return descriptionText;
            }
            set
            {
                if (this.descriptionText != value)
                {
                    this.descriptionText = value;
                    this.RaisePropertyChanged("DescriptionText");
                }
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.RaisePropertyChanged("Description");
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
    }
}
