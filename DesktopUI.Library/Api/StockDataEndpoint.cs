using DesktopUI.Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using YahooFinanceApi;

namespace DesktopUI.Library.Api
{
    public class StockDataEndpoint: IStockDataEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public StockDataEndpoint(IApiHelper apiHelper)
        {
           _apiHelper = apiHelper;
        }

        public async Task<List<OhlcStockModel>> GetDashboardCharts(string ticker)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://yfapi.net/");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", "9l4Vorm2Kb7Z5HeFpMN8raQTY4X8z0HL9bMNChR6");
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await httpClient.GetAsync($"v8/finance/chart/{ticker}?range1mo=&region=US&interval=15m&lang=en");

            var responseBody = await response.Content.ReadAsStringAsync();

            var data = (JObject)JsonConvert.DeserializeObject(responseBody);

            //var test = data.SelectToken("chart.result[0].indicators.quote[0]");

            var open = data.SelectToken("chart.result[0].indicators.quote[0].open").ToList();

            var high = data.SelectToken("chart.result[0].indicators.quote[0].high").ToList();

            var low = data.SelectToken("chart.result[0].indicators.quote[0].low").ToList();

            var close = data.SelectToken("chart.result[0].indicators.quote[0].close").ToList();

            var volume = data.SelectToken("chart.result[0].indicators.quote[0].volume").ToList();

            List<OhlcStockModel> stocks = new List<OhlcStockModel>();

            for(int i = 0; i < close.Count; i++)
            {
                stocks.Add(new OhlcStockModel
                {
                    Open = open[i].ToObject<decimal>(),
                    Close = close[i].ToObject<decimal>(),
                    High = high[i].ToObject<decimal>(),
                    Low = low[i].ToObject<decimal>(),
                    Volume = volume[i].ToObject<long>()
                });
            }

            return stocks;

        }


        //public async Task<List<StockDataModel>> GetDowData(string ticker)
        //{
        //    var p = Period.Daily;
        //    DateTime startDate = new DateTime(2022, 01, 01);
        //    DateTime endDate = DateTime.Now;

        //    var hist = await Yahoo.GetHistoricalAsync(ticker, startDate, endDate, p);

        //    List<StockDataModel> models = new List<StockDataModel>();
        //    foreach (var r in hist)
        //    {
        //        models.Add(new StockDataModel
        //        {
        //            Ticker = ticker,
        //            Date = r.DateTime.ToString("yyyy-MM-dd"),
        //            Open = (double)r.Open,
        //            High = (double)r.High,
        //            Low = (double)r.Low,
        //            Close = (double)r.Close,
        //            AdjustedClose = r.AdjustedClose,
        //            Volume = r.Volume
        //        });
        //    }
        //    return models;

       // }


    }
}
