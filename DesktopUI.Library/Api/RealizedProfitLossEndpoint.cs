using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class RealizedProfitLossEndpoint: IRealizedProfitLossEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public RealizedProfitLossEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }   

        public async Task PostProfitLoss(decimal realizedProfitLoss)
        {
            var realizedPL = new UpdateRealizedPLModel
            {
                RealizedProfitLoss = realizedProfitLoss
            };

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/realizedpl", realizedPL))
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

        public async Task<List<RealizedPLChartModel>> LoadHistory()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/realizedpl"))
            {
                if(response.IsSuccessStatusCode)
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
    }
}
