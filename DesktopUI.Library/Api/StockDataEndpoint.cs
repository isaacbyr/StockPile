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

            var response = await httpClient.GetAsync($"v8/finance/chart/{ticker}?range=3mo&region=US&interval=1d&lang=en");

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

        public async Task<StockDashboardDataModel> GetStockDashboardData(string ticker)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://yfapi.net/");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", "9l4Vorm2Kb7Z5HeFpMN8raQTY4X8z0HL9bMNChR6");
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await httpClient.GetAsync($"v6/finance/quote?region=US&lang=en&symbols={ticker}");

            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var stock = new StockDashboardDataModel
                {
                    Ticker = ticker.ToUpper(),
                    MarketPrice = data.SelectToken("quoteResponse.result[0].regularMarketPrice").ToString(),
                    PercentChanged = data.SelectToken("quoteResponse.result[0].regularMarketChangePercent").ToString()
                };

                return stock;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<List<StockDashboardDataModel>> GetDailyGainers()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://yfapi.net/");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", "9l4Vorm2Kb7Z5HeFpMN8raQTY4X8z0HL9bMNChR6");
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await httpClient.GetAsync("ws/screeners/v1/finance/screener/predefined/saved?count=25&scrIds=day_gainers");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var stocks = new List<StockDashboardDataModel>();

                foreach (var s in data.SelectToken("finance.result[0].quotes"))
                {
                    var stock = new StockDashboardDataModel
                    {
                        Ticker = s.SelectToken("symbol").ToString(),
                        MarketPrice = s.SelectToken("regularMarketPrice").ToString(),
                        PercentChanged = s.SelectToken("regularMarketChangePercent").ToString()
                    };
                    stocks.Add(stock);
                }
                return stocks;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<List<StockDashboardDataModel>> GetMultipleStockDashboardData(string query)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://yfapi.net/");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", "9l4Vorm2Kb7Z5HeFpMN8raQTY4X8z0HL9bMNChR6");
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await httpClient.GetAsync($"v6/finance/quote?region=US&lang=en&symbols={query}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var stocks = new List<StockDashboardDataModel>();

                
                foreach(var s in data.SelectToken("quoteResponse.result"))
                {
                    var stock = new StockDashboardDataModel
                    {
                        Ticker = s.SelectToken("symbol").ToString(),
                        MarketPrice = s.SelectToken("regularMarketPrice").ToString(),
                        PercentChanged = s.SelectToken("regularMarketChangePercent").ToString()
                    };
                    stocks.Add(stock);
                } 

                return stocks;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
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
