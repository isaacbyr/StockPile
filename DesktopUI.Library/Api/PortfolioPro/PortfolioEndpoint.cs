using DesktopUI.Library.Models;
using DesktopUI.Library.Models.Shared;
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

        public async Task<List<TopHoldingsModel>> LoadTopHoldings()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/portfolio/topholdings"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<TopHoldingsModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
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

        public async Task<PortfolioStockDashboardModel> GetPortfolioStock(string ticker)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync($"/api/portfolio/{ticker}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<PortfolioStockDashboardModel>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task UpdatePortfolioBuy(PortfolioModel stock)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/portfolio/buy", stock))
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

        public async Task<decimal> UpdatePortfolioSell(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/portfolio/sell", stock))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<decimal>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task PostStock(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/portfolio", stock))
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

        public async Task<decimal> UpdateAndDeletePortfolio(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/portfolio/delete", stock))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<decimal>();
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
