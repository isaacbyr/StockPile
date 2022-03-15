using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace DesktopUI.ViewModels.Twitter
{
    public class TwitterScreenerViewModel : Screen
    {
        private static string consumerKey = "95uNk0DJUl8f8mMc13ZRSy1fd";
        private static string consumerSecret = "c46EHeHdIUfZKb3GDDRcov1TrJUDqPoALwmzvyhoFtRx3DjDZD";
        private static string userAccessToken = "954379468847570944-ydZPSSPL7ALbXt8IfuHsqlWPqJYk14F";
        private static string userAccessSecret = "65jIE2lcJ3NgCX9ChrqcsXr14TgB8eZ6yt9xqrcrzKmEk";
        public static TwitterClient userClient;

        public TwitterScreenerViewModel(IAuthenticationRequest authenticationRequest)
        {
            
        }

        protected override void OnViewLoaded(object view)
        {
            InitalizeAuthTwitter();
        }

        private async void InitalizeAuthTwitter()
        {
            var userCredentials = new TwitterCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
            userClient = new TwitterClient(userCredentials);

            await SearchTweets();
        }

        private async Task SearchTweets()
        {
            try
            {
                var searchIterator = userClient.SearchV2.GetSearchTweetsV2Iterator("hello");
                while (!searchIterator.Completed)
                {
                    var searchPage = await searchIterator.NextPageAsync();
                    var searchResponse = searchPage.Content;
                    var tweets = searchResponse.Tweets;
                    // ...
                }

            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
    }
}
