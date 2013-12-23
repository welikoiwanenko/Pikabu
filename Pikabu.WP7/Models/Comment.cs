using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pikabu.Models
{
    public class Comment : INotifyPropertyChanged
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

        private string nick;
        public string Nick
        {
            get
            {
                return nick;
            }
            set
            {
                if (this.nick != value)
                {
                    this.nick = value;
                    this.RaisePropertyChanged("Nick");
                }
            }
        }

        private string answer;
        public string Answer
        {
            get
            {
                return answer;
            }
            set
            {
                if (this.answer != value)
                {
                    this.answer = value;
                    this.RaisePropertyChanged("Answer");
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (this.date != value)
                {
                    this.date = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }

        private string value;
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        private int margin;
        public int Margin
        {
            get
            {
                return margin;
            }
            set
            {
                if (this.margin != value)
                {
                    this.margin = value;
                    this.RaisePropertyChanged("Margin");
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
