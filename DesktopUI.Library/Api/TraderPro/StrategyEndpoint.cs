using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class StrategyEndpoint : IStrategyEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public StrategyEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<int> PostStrategy(StrategyModel strategy)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/strategy/new", strategy))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<ResponseModel> PostStrategyStock(StrategyStockModel strategyStock)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/strategy/stock", strategyStock))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<ResponseModel>();
                    return result;
                }
                else
                {
                    var result = new ResponseModel
                    {
                        Header = "Error",
                        Message = response.ReasonPhrase
                    };
                    return result;
                }
            }
        }
    }
}
