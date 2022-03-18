using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class TWSTradingEndpoint : ITWSTradingEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public TWSTradingEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostTWSStategy(TWSTradeModel trade)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/twstrading", trade))
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

        public async Task<List<TWSTradeModel>> LoadStrategies()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/twstrading"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<TWSTradeModel>>();
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
