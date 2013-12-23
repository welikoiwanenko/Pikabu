using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pikabu.Helpers;
using Pikabu.Models;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Pikabu
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        string PostID;
        ObservableCollection<Comment> comments;
        string html;
        Stack<Uri> history = new Stack<Uri>();
        Uri current = null;
        bool isRefreshBouttonPressed = false;

        public CommentsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator) //если активация страницы произошла из самого приложения
            {
                NavigationContext.QueryString.TryGetValue("PostID", out PostID);
            }
            else //если из меню многозадачности
            {
                PostID = PhoneApplicationService.Current.State["PostID"].ToString();
            }
            foreach (var item in App.HotStoryVM.StoriesSource)
            {
                if (item.ID == PostID)
                {
                    title.Text = "\"" + item.Title + "\"";
                    break;
                }
            }
            SetBrowserContent();
        }

        async private void SetBrowserContent()
        {
            html = string.Empty;
            string a = "http://pikabu.ru/generate_xml_comm.php?id=" + PostID;
            string x = await new WebClient().DownloadStringTaskAsync("http://pikabu.ru/generate_xml_comm.php?id=" + PostID);
            //XDocument xml = XDocument.Parse(x);
            //XDocument xml = new XDocument();
            
            XDocument xml = XDocument.Parse(x);
            var result = from n in xml.Descendants("comment")
                         select new Comment()
                         {
                             ID = n.Attribute("id").Value,
                             Date = DateTime.Parse(n.Attribute("date").Value),
                             Answer = n.Attribute("answer").Value,
                             Nick = n.Attribute("nick").Value,
                             Rating = n.Attribute("rating").Value,
                             Value = n.Value,
                             Margin = 0
                         };
            comments = new ObservableCollection<Comment>();
            foreach (var item in result)
            {
                comments.Add(item);
            }
            foreach (var item in comments)
            {
                if (item.Answer != "0")
                {
                    foreach (var i in comments)
                    {
                        if (i.ID==item.Answer)
                        {
                            item.Margin = i.Margin + 50;
                            break;
                        }
                    }
                }
                if (item.Value.Contains("class=\"c_img\""))
                {
                    item.Value = item.Value.Insert(item.Value.IndexOf("<img class=\"c_img\""), "</br>");
                }
            }
            html = @"
                        <html>
                            <head>
                                <style>

                                    .container {
                                        font-size: 50px;
                                        height:auto;
                                    }
                                    img {
                                      border-width: 0;
                                    }
                                    .menu {
                                        float: left;
                                        width: auto;
                                        height: auto;
                                        color: rgb(133, 133, 133);
                                    }
                                    .content {
                                        width: 100%;
                                        height: auto;
                                        text-align:right;
                                    }

                                    .footer {
                                        font-size: 15px;
                                        height: auto;
                                        width: auto;
                                    }
                                </style>
                            <script type='text/javascript'>
                                function goBack()
                                    {
                                        history.go(-1);
                                    }    
                            </script>
                            </head>

                            <body style='margin-left:35; margin-right:35'>";
    
            foreach (var item in comments)
            {
                html += @"
                            <div class='container' style='margin:0 0 80 " + item.Margin + @"'>
                                    <div class='menu'>
                                        <p>" + item.Nick + @"</p>
                                    </div>";
                if (item.Rating=="0")
                {
                    html += @"<div class='content'>
                        <p style='color:rgb(133, 133, 133)'>" + item.Rating + @"</p>
                    </div>";
                }
                else if(item.Rating.Contains("-"))
                {
                    html += @"<div class='content'>
                        <p style='color:rgb(255, 0, 0)'>" + item.Rating + @"</p>
                    </div>";
                }
                else
                {
                    html += @"<div class='content'>
                        <p style='color:rgb(0, 200, 74)'>" + item.Rating + @"</p>
                    </div>";
                }
                                    
                html+=  @"<div class='footer'>
                                        <p>" + item.Value.Replace("\\\"","\"") + @"</p>
                                    </div>
                                </div>
                            ";
            }
            html+=@"
                        </body>
                        </html>
                        ";
            webBrowser.NavigateToString(html);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!e.IsNavigationInitiator)
            {
                PhoneApplicationService.Current.State["PostID"] = PostID;
            }
        }



        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //try
            //{
            //    SetBrowserContent();
            //    webBrowser.NavigateToString("<html><body onLoad=\"history.go(-2);\"></body></html>");
            //}
            //catch
            //{
            //    base.OnBackKeyPress(e);
            //}
            //e.Cancel = true;
            base.OnBackKeyPress(e);

            //if (!isPerformingCloseOperation)
            {
                if (history.Count > 0)
                {
                    Uri destination = history.Peek();
                    //webBrowser.Navigate(destination);
                    SetBrowserContent();
                    isRefreshBouttonPressed = true;
                    // What about using script and going history.back? 
                    // you can do it, but 
                    // I rather use that to keep ‘track’ consistently with our stack 
                    e.Cancel = true;
                }
            } 
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            SetBrowserContent();
            isRefreshBouttonPressed = true;
        }

        private void webBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Uri previous = null;
            if (history.Count > 0)
            {
                previous = history.Peek();
            }

            // This assumption is NOT always right. 
            // if the page had a forward reference that creates a loop (e.g. A->B->A ), 
            // we would not detect it, we assume it is an A -> B -> back () 
            if (e.Uri == previous)
            {
                history.Pop();
            }
            else
            {
                if (current != null)
                    history.Push(current);
            }
            current = e.Uri;

            if (history.Count == 0||isRefreshBouttonPressed)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                isRefreshBouttonPressed = false;
            }
            else
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
            }
        }
    }
}