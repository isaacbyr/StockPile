﻿using DesktopUI.Library.Models;
using DesktopUI.Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class TradePortfolioEndpoint: ITradePortfolioEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public TradePortfolioEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<TopHoldingsModel>> LoadTopHoldings()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/tradesportfolio/topholdings"))
            {
                if (response.IsSuccessStatusCode)
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
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/tradesportfolio"))
            {
                if (response.IsSuccessStatusCode)
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
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync($"/api/tradesportfolio/{ticker}"))
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

        public async Task PostStock(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/tradesportfolio", stock))
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

        public async Task UpdatePortfolioBuy(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/tradesportfolio/buy", stock))
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

        public async Task<decimal> UpdatePortfolioSell(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/tradesportfolio/sell", stock))
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

        public async Task<decimal> UpdateAndDeletePortfolio(PortfolioModel stock)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/tradesportfolio/delete", stock))
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
    }
}
