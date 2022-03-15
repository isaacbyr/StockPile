using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class FriendRequestEndpoint: IFriendRequestEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public FriendRequestEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<FriendModel>> LoadFriendRequests()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/friendrequests"))
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

        public async Task DeleteRequest(string followerId)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.DeleteAsync($"/api/friendrequests/{followerId}"))
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

        public async Task PostRequest(FriendRequestModel request)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/friendrequests", request))
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
