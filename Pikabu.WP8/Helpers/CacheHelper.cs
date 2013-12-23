using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Pikabu.Helpers
{
    public delegate void FavoritesCreationEventHandler();
    public delegate void FavoritesDeletionEventHandler();
    public static class CacheHelper
    {

        /// <summary>
        /// Событие окончания добавления в избранное
        /// </summary>
        public static event FavoritesCreationEventHandler FavoritesCreationCompleted;

        /// <summary>
        /// Событие срыва добавления в избранное
        /// </summary>
        public static event FavoritesCreationEventHandler FavoritesCreationFailed;

        /// <summary>
        /// Событие окончания удаления из избранного
        /// </summary>
        public static event FavoritesDeletionEventHandler FavoritesDeletionCompleted;

        /// <summary>
        /// Событие окончания удаления всего избранного
        /// </summary>
        public static event FavoritesDeletionEventHandler FavoritesDeletionAllCompleted;


        /// <summary>
        /// Метод для сохранения поста в IsolatedStorage
        /// </summary>
        /// <param name="story">История для сохранения</param>
        /// <param name="postType">Тип поста для сохранения</param>
        public static void CreateOfflinePost(OnlineStory story, OfflinePostType postType)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (!store.DirectoryExists(postType.ToString()))
            {
                store.CreateDirectory(postType.ToString());
            }
            if (!store.DirectoryExists(postType.ToString() + "\\" + story.ID))
            {
                store.CreateDirectory(postType.ToString() + "\\" + story.ID);
            }
            switch (story.Type)
            {
                case StoryType.Image:
                    BitmapImage img = new BitmapImage(story.Image) { CreateOptions = BitmapCreateOptions.None };
                    img.ImageOpened += (s, e) =>
                    {
                        using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + ".html", FileMode.OpenOrCreate, store))
                        {
                            var fileWriter = new StreamWriter(stream);
                            fileWriter.Write("<html><body></br> <image style='width:100%;margin:12' src='" + story.ID + ".jpg'></img> </br> <p style='margin:12'>" + OtherHelpers.ConvertExtendedASCII(story.Description) + "</p></body></html>");
                            fileWriter.Close();
                        }
                        using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + "_title.txt", FileMode.OpenOrCreate, store))
                        {
                            var fileWriter = new StreamWriter(stream);
                            fileWriter.Write(story.Title);
                            fileWriter.Close();
                        }
                        using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + "_desc.txt", FileMode.OpenOrCreate, store))
                        {
                            var fileWriter = new StreamWriter(stream);
                            fileWriter.Write(story.Description);
                            fileWriter.Close();
                        }
                        WriteableBitmap bitmap = new WriteableBitmap((BitmapImage)s);
                        using (var stream = store.OpenFile(postType.ToString() + "\\" + story.ID + "\\" + story.ID + ".jpg", FileMode.Create))
                        {
                            Extensions.SaveJpeg(bitmap, stream,
                                bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
                        }
                        if (FavoritesCreationCompleted != null)
                        {
                            FavoritesCreationCompleted();
                        }
                    };
                    img.ImageFailed += (s, e) =>
                    {
                        if (FavoritesCreationFailed != null)
                        {
                            FavoritesCreationFailed();
                        }
                    };
                    break;
                case StoryType.Text:
                    using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + ".html", FileMode.OpenOrCreate, store))
                    {
                        var fileWriter = new StreamWriter(stream);
                        fileWriter.Write("<html><body> </br> <p style='margin:12'>" + OtherHelpers.ConvertExtendedASCII(story.DescriptionText) + "</p></body></html>");

                        fileWriter.Close();
                    }
                    using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + "_title.txt", FileMode.OpenOrCreate, store))
                    {
                        var fileWriter = new StreamWriter(stream);
                        fileWriter.Write(story.Title);
                        fileWriter.Close();
                    }
                    using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + story.ID + "\\" + story.ID + "_desc.txt", FileMode.OpenOrCreate, store))
                    {
                        var fileWriter = new StreamWriter(stream);
                        fileWriter.Write(story.Description);
                        fileWriter.Close();
                    }
                    if (FavoritesCreationCompleted != null)
                    {
                        FavoritesCreationCompleted();
                    }
                    //
                    break;
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Метод для получения списка постов из IsolatedStorage
        /// </summary>
        /// <param name="postType">Тип получаемых постов</param>
        /// <returns>Списсок постов OfflineStory</returns>
        public static IList<OfflineStory> OpenOfflinePosts(OfflinePostType postType)
        {
            List<OfflineStory> stories = new List<OfflineStory>();
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.DirectoryExists(postType.ToString()))
            {
                var directoryNames = store.GetDirectoryNames(postType.ToString() + "\\*");
                foreach (var item in directoryNames)
                {
                    if(store.FileExists(postType.ToString() + "\\" + item + "\\" + item + "_desc.txt")&&store.FileExists(postType.ToString() + "\\" + item + "\\" + item + "_title.txt"))
                    {
                        OfflineStory os = new OfflineStory();
                        if (store.FileExists(postType.ToString() + "\\" + item + "\\" + item + ".jpg"))
                        {
                            os.ThumbFileName = postType.ToString() + "\\" + item + "\\" + item + ".jpg";
                        }
                        using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + item + "\\" + item + "_desc.txt", FileMode.Open, store))
                        {
                            var fileReader = new StreamReader(stream);
                            string a = fileReader.ReadToEnd();
                            os.Description = a;
                            fileReader.Close();
                            fileReader.Dispose();
                            stream.Dispose();
                        }
                        using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + item + "\\" + item + "_title.txt", FileMode.Open, store))
                        {
                            var fileReader = new StreamReader(stream);
                            string a = fileReader.ReadToEnd();
                            os.Title = a;
                            fileReader.Close();
                            fileReader.Dispose();
                            stream.Dispose();
                        }
                        os.ID = item;
                        os.Type = StoryType.Offline;
                        stories.Add(os);
                    }
                    else
                    {
                        DeleteOfflinePost(item, postType);
                    }
                }
            }
            return stories;
        }

        public static string GetOfflinePostTitle(string id, OfflinePostType postType)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists(postType.ToString() + "\\" + id + "\\" + id + ".html"))
            {
                using (var stream = new IsolatedStorageFileStream(postType.ToString() + "\\" + id + "\\" + id + "_title.txt", FileMode.Open, store))
                {
                    var fileReader = new StreamReader(stream);
                    string a = fileReader.ReadToEnd();
                    fileReader.Close();
                    return a;
                }
            }
            return "";
        }

        public static void DeleteOfflinePost(string id, OfflinePostType postType)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.DirectoryExists(postType.ToString() + "\\" + id))
            {
                var a = store.GetFileNames(postType.ToString() + "\\" + id + "\\*");
                foreach (var file in store.GetFileNames(postType.ToString() + "\\" + id + "\\*"))
                {
                    store.DeleteFile(postType.ToString() + "\\" + id + "\\" + file);
                }
                store.DeleteDirectory(postType.ToString() + "\\" + id);
            }
            if (FavoritesDeletionCompleted != null)
            {
                FavoritesDeletionCompleted();
            }
        }

        public static void DeleteAllOfflinePosts(OfflinePostType postType)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.DirectoryExists(postType.ToString()))
            {
                var directoryNames = store.GetDirectoryNames(postType.ToString() + "\\*");
                try
                {
                    foreach (var item in directoryNames)
                    {
                        DeleteOfflinePost(item, postType);
                    }
                    if (FavoritesDeletionAllCompleted != null)
                    {
                        FavoritesDeletionAllCompleted();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public static IList<string> GetOfflinePostsID(OfflinePostType postType)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.DirectoryExists(postType.ToString()))
            {
                var directoryNames = store.GetDirectoryNames(postType.ToString() + "\\*");
                return directoryNames;
            }
            return new List<string> { "error" };
        }

    }
    public enum OfflinePostType
    {
        Favorites, OfflineRead
    }
}
