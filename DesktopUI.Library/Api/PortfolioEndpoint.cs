using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class PortfolioEndpoint: IPortfolioEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public PortfolioEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<PortfolioStockDashboardModel>> LoadPortfolioStocks()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/portfolio"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<PortfolioStockDashboardModel>>();
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
