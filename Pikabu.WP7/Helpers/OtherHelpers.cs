using Pikabu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pikabu.Helpers
{
    public static class OtherHelpers
    {
        /// <summary>
        /// Метод для правильного отображения русской кодировки
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string ConvertExtendedASCII(string HTML)
        {
            string retVal = "";
            char[] s = HTML.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }
        public static ObservableCollection<OnlineStory> GetBest20(this ObservableCollection<OnlineStory> collection)
        {
            List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>();
            foreach (var item in collection)
            {
                myList.Add(new KeyValuePair<string, int>(item.ID, Convert.ToInt32(item.Rating)));
            }
            myList.Sort((firstPair, nextPair) =>
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            });
            myList.Reverse();
            myList.RemoveRange(20, myList.Count - 20);
            ObservableCollection<OnlineStory> best20 = new ObservableCollection<OnlineStory>();
            foreach (var pair in myList)
            {
                foreach (var item in collection)
                {
                    if (pair.Key == item.ID && pair.Value.ToString() == item.Rating)
                    {
                        best20.Add(item);
                    }
                }
            }
            return best20;
        }
        async public static Task<string> LoadPageAsync(string url)
        {
            var client = new WebClient();
            string page = await client.DownloadStringTaskAsync(url);
            return page;
        }
    }
}
