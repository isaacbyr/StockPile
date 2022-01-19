using DesktopUI.Library.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class ApiHelper : IApiHelper
    {
        private HttpClient apiClient;
        private readonly ILoggedInUserModel _loggedInUser;

        public ApiHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        public HttpClient ApiClient
        {
            get
            {
                return apiClient;
            }
        }
        private void InitializeClient()
        {
            // string to the api we want to access. Saved in settings so that it can be changed easier
            string api = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUserModel> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var authenticatedUser = JsonConvert.DeserializeObject<AuthenticatedUserModel>(result);
                    return authenticatedUser;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public async Task GetLoggedInUserInfo(string token)
        {
            apiClient.DefaultRequestHeaders.Clear();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await apiClient.GetAsync("/api/user"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<LoggedInUserModel>>();
                    _loggedInUser.FirstName = result[0].FirstName;
                    _loggedInUser.LastName = result[0].LastName;
                    _loggedInUser.Email = result[0].Email;
                    _loggedInUser.Id = result[0].Id;
                    //    _loggedInUser.Token = token;
                    return;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public void Logout()
        {
            apiClient.DefaultRequestHeaders.Clear();
        }


        public async Task RegisterUser(RegisterUserModel user)
        {

            apiClient.DefaultRequestHeaders.Clear();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpResponseMessage response = await apiClient.PostAsJsonAsync("/api/account/register", user))
            {
                if (response.IsSuccessStatusCode)
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
