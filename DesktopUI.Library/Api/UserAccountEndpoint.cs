using DesktopUI.Library.Models;
using Newtonsoft.Json;
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

        public async Task<decimal> LoadTradesAccountBalance()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/useraccount/tradesbalance"))
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

        public async Task<decimal> LoadPortfolioAccountBalance()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/useraccount/balance"))
            {
                if (response.IsSuccessStatusCode)
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

        public async Task UpdatePortfolioAccountBalance(decimal cashAmount)
        {
            var amount = new UpdateUserAccountModel
            {
                Amount = cashAmount
            };

            using(HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/useraccount/updatebalance/portfolio", amount))
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

        public async Task<decimal> UpdateAfterSale(decimal realizedProfitLoss, decimal cashAmount)
        {
            var amount = new UpdateUserAccountModel
            {
                Amount = cashAmount,
                RealizedProfitLoss = realizedProfitLoss
            };

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/useraccount/sale", amount))
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

        public async Task PostNewUserAccount(UserAccountModel userAccount)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/useraccount", userAccount))
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

        public async Task UpdateTradesAccountBalance(decimal cashAmount)
        {
            var amount = new UpdateUserAccountModel
            {
                Amount = cashAmount
            };

            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync("/api/useraccount/updatebalance/trades", amount))
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
