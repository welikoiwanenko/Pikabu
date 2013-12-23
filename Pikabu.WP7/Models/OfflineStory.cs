using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Pikabu.Models
{
    public class OfflineStory : Story
    {
        private string _thumbFileName;
        public string ThumbFileName
        {
            get
            {
                return _thumbFileName;
            }
            set
            {
                _thumbFileName = value;
                RaisePropertyChanged("ThumbFileName");
                RaisePropertyChanged("ThumbImage");
            }
        }

        public BitmapImage ThumbImage
        {
            get
            {
                BitmapImage image = new BitmapImage();
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                string isoFilename = ThumbFileName;
                try
                {
                    var stream = isoStore.OpenFile(isoFilename, System.IO.FileMode.Open);
                    image.SetSource(stream);
                    stream.Close();
                }
                catch (Exception)
                {

                }
                
                return image;
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
