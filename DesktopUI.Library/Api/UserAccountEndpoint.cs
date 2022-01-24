using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class UserAccountEndpoint : IUserAccountEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public UserAccountEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<UserPortfolioOverviewModel> GetPortfolioOverview()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/useraccount"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<UserPortfolioOverviewModel>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
