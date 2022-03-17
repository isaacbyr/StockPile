using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class TradeRealizedPLEndpoint : ITradeRealizedPLEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public TradeRealizedPLEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostProfitLoss(decimal realizedProfitLoss)
        {
            var realizedPL = new UpdateRealizedPLModel
            {
                RealizedProfitLoss = realizedProfitLoss
            };

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/traderealizedpl", realizedPL))
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

        public async Task<List<RealizedPLChartModel>> LoadHistory()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/traderealizedpl"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<RealizedPLChartModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<LeaderboardModel>> LoadRealizedPL(string id)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync($"/api/traderealizedpl/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<LeaderboardModel>>();
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
