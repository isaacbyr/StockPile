using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class FriendsEndpoint: IFriendsEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public FriendsEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<FriendModel>> LoadFriends()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/friends"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<FriendModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task PostFriendship(string followerId)
        {
            var newfriendship = new NewFriendshipModel
            {
                FollowerId = followerId
            };

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/friends", newfriendship))
            {
                if(response.IsSuccessStatusCode)
                {
                    return;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
