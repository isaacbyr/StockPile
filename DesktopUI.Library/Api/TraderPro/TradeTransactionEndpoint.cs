using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class TradeTransactionEndpoint: ITradeTransactionEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public TradeTransactionEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostTransaction(TransactionModel transaction)
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/tradetransaction", transaction))
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

        public async Task<List<SocialDashboardDataModel>> LoadTransactionsById(string id)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync($"/api/tradetransaction/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<SocialDashboardDataModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<TransactionChartData>> LoadChartData()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/tradetransaction/chart"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<TransactionChartData>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public async Task<List<TransactionModel>> LoadTransactions()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/tradetransaction"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<TransactionModel>>();
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
