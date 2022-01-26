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

        public async Task<(List<OhlcStockModel>, string, string)> GetDashboardCharts(string ticker, string range, string interval)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://yfapi.net/");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", "9l4Vorm2Kb7Z5HeFpMN8raQTY4X8z0HL9bMNChR6");
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await httpClient.GetAsync($"v8/finance/chart/{ticker}?range={range}&region=US&interval={interval}&lang=en");

            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                //var test = data.SelectToken("chart.result[0].indicators.quote[0]");

                var timestamp = data.SelectToken("chart.result[0].timestamp").ToList();

                List<string> dates = new List<string>();

                foreach (var time in timestamp)
                {
                    var ts = (int)time;
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();
                    string formattedDate = dt.ToString("dd-MMM");
                    dates.Add(formattedDate);
                }

                var symbol = data.SelectToken("chart.result[0].meta.symbol").ToString();

                var marketPrice = data.SelectToken("chart.result[0].meta.regularMarketPrice").ToString();

                var open = data.SelectToken("chart.result[0].indicators.quote[0].open").ToList();

                var high = data.SelectToken("chart.result[0].indicators.quote[0].high").ToList();

                var low = data.SelectToken("chart.result[0].indicators.quote[0].low").ToList();

                var close = data.SelectToken("chart.result[0].indicators.quote[0].close").ToList();

                var volume = data.SelectToken("chart.result[0].indicators.quote[0].volume").ToList();

                List<OhlcStockModel> stocks = new List<OhlcStockModel>();

                for (int i = 0; i < close.Count; i++)
                {
                    stocks.Add(new OhlcStockModel
                    {
                        Open = open[i].ToObject<decimal>(),
                        Close = close[i].ToObject<decimal>(),
                        High = high[i].ToObject<decimal>(),
                        Low = low[i].ToObject<decimal>(),
                        Volume = volume[i].ToObject<long>(),
                        Date = dates[i]
                    });
                }

                return (stocks, symbol, marketPrice);
            }

           else
            {
                throw new Exception(response.ReasonPhrase);
            }

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

                decimal price;
                decimal.TryParse(data.SelectToken("quoteResponse.result[0].regularMarketPrice").ToString(), out price);
                var stock = new StockDashboardDataModel
                {
                    Ticker = ticker.ToUpper(),
                    MarketPrice = price,
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
                    decimal price;
                    decimal.TryParse(s.SelectToken("regularMarketPrice").ToString(), out price);
                    var stock = new StockDashboardDataModel
                    {
                        Ticker = s.SelectToken("symbol").ToString(),
                        MarketPrice = price,
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
                    decimal temp;
                    decimal.TryParse(s.SelectToken("regularMarketPrice").ToString(), out temp);
                    var stock = new StockDashboardDataModel
                    {
                        Ticker = s.SelectToken("symbol").ToString(),
                        MarketPrice = temp,
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

        public async Task<CompanyOverviewModel> GetCompanyOverview(string ticker)
        {
            string API_KEY = "RHMBFDRST81LETMU";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.alphavantage.co/");

            var response = await httpClient.GetAsync($"query?function=OVERVIEW&symbol={ticker}&apikey={API_KEY}");

            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var companyOverview = new CompanyOverviewModel
                {
                    Symbol = data.SelectToken("Symbol").ToString(),
                    Sector = data.SelectToken("Sector").ToString(),
                    MarketCapitalization = data.SelectToken("MarketCapitalization").ToString(),
                    EBITDA = data.SelectToken("EBITDA").ToString(),
                    PERatio = data.SelectToken("PERatio").ToString(),
                    PEGRatio = data.SelectToken("PEGRatio").ToString(),
                    DividendYield = data.SelectToken("DividendYield").ToString(),
                    EPS = data.SelectToken("EPS").ToString(),
                    Beta = data.SelectToken("Beta").ToString(),
                    ForwardPE = data.SelectToken("ForwardPE").ToString(),
                    TrailingPE = data.SelectToken("TrailingPE").ToString(),
                    SharesOutstanding = data.SelectToken("SharesOutstanding").ToString()
                };

                return companyOverview;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }


    }
}
