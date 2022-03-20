using DesktopUI.Library.Models.TraderPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public class PolygonDataEndpoint: IPolygonDataEndpoint
    {
        public async Task<List<double>> LoadMAData(string ticker, int interval, string startTimestamp, string endTimestamp, int lastResults)
        {
            string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.polygon.io/");

            var response = await httpClient.GetAsync($"v2/aggs/ticker/{ticker}/range/{interval}/minute/{startTimestamp}/{endTimestamp}?adjusted=true&sort=asc&limit=50000&apiKey={API_KEY}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var closes = new List<double>();

                foreach (var r in data.SelectToken("results"))
                {
                    var close = (double)r.SelectToken("c");
                    var stringTs = (string)r.SelectToken("t");

                    var dt = ConvertTimestampToDate(stringTs);

                    var lower = new DateTime(2011, 9, 14, 6, 30, 00);
                    var upper = new DateTime(2011, 9, 14, 13, 00, 00);

                    if (dt.TimeOfDay >= lower.TimeOfDay && dt.TimeOfDay <= upper.TimeOfDay)
                    {           
                        closes.Add(close);
                    }

                }

                closes = closes.Skip(Math.Max(0, closes.Count - lastResults)).Take(lastResults).ToList();

                return closes;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }


        public async Task<(List<double>, List<double>, List<double>, List<double>, List<DateTime>)> LoadTradeData(string ticker,int interval, string startTimestamp, string endTimestamp)
        {
            string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.polygon.io/");

            var response = await httpClient.GetAsync($"v2/aggs/ticker/{ticker}/range/{interval}/minute/{startTimestamp}/{endTimestamp}?adjusted=true&sort=asc&limit=50000&apiKey={API_KEY}");

            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var opens = new List<double>();
                var highs = new List<double>();
                var closes = new List<double>();
                var lows = new List<double>();
                var dates = new List<DateTime>();

                int index = 0;
                int startIndex = 0;
                int endIndex = (int)data.SelectToken("resultsCount");
                foreach (var r in data.SelectToken("results"))
                {
                    var open = (double)r.SelectToken("o");
                    var high = (double)r.SelectToken("h");
                    var low = (double)r.SelectToken("l");
                    var close = (double)r.SelectToken("c");
                    var stringTs = (string)r.SelectToken("t");

                    var dt = ConvertTimestampToDate(stringTs);

                    if(dt.Hour == 6 && dt.Minute == 30)
                    {
                        startIndex = index;
                    }
                    if(dt.Hour == 13 && dt.Minute == 0)
                    {
                        endIndex = index;
                    }


                    opens.Add(open);
                    highs.Add(high);
                    closes.Add(close);
                    lows.Add(low);
                    dates.Add(dt);

                    index++;
                }

                var removeCount = opens.Count - endIndex;

                opens.RemoveRange(endIndex, removeCount);
                opens.RemoveRange(0, startIndex);
                highs.RemoveRange(endIndex, removeCount);
                highs.RemoveRange(0, startIndex);
                lows.RemoveRange(endIndex, removeCount);
                lows.RemoveRange(0, startIndex);
                closes.RemoveRange(endIndex, removeCount);
                closes.RemoveRange(0, startIndex);
                dates.RemoveRange(endIndex, removeCount);
                dates.RemoveRange(0, startIndex);

                return (opens, highs, lows, closes, dates);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<(List<double>, List<double>, List<double>, List<double>, List<DateTime>)> LoadChartData(string ticker, int interval, string startTimestamp, string endTimestamp)
        {
            string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.polygon.io/");

            var response = await httpClient.GetAsync($"v2/aggs/ticker/{ticker}/range/{interval}/minute/{startTimestamp}/{endTimestamp}?adjusted=true&sort=asc&limit=50000&apiKey={API_KEY}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var opens = new List<double>();
                var highs = new List<double>();
                var closes = new List<double>();
                var lows = new List<double>();
                var dates = new List<DateTime>();

                foreach (var r in data.SelectToken("results"))
                {
                    var open = (double)r.SelectToken("o");
                    var high = (double)r.SelectToken("h");
                    var low = (double)r.SelectToken("l");
                    var close = (double)r.SelectToken("c");
                    var stringTs = (string)r.SelectToken("t");

                    var dt = ConvertTimestampToDate(stringTs);

                    var lower = new DateTime(2011, 9, 14, 6, 30, 00);
                    var upper = new DateTime(2011, 9, 14, 13, 00, 00);

                    if(dt.TimeOfDay >= lower.TimeOfDay && dt.TimeOfDay <= upper.TimeOfDay)
                    {
                        opens.Add(open);
                        highs.Add(high);
                        closes.Add(close);
                        lows.Add(low);
                        dates.Add(dt);
                    }

                }

                return (opens, highs, lows, closes, dates);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        private DateTime ConvertTimestampToDate(string stringTs)
        {

            int ts;
            int.TryParse(stringTs.Substring(0, 10), out ts);
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();

            return date;

        }

        public async Task<List<RecentTradeModel>> LoadRecentTrades(string ticker, double timestamp)
        {
            string convTimestamp = timestamp.ToString(".################################################");
            string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.polygon.io/");

            var response = await httpClient.GetAsync($"vX/trades/{ticker}?timestamp.lt={convTimestamp}&order=desc&sort=timestamp&limit=10000&apiKey={API_KEY}");
            
            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var trades = new List<RecentTradeModel>();

                foreach(var s in data.SelectToken("results"))
                {
                    var price = (double)s.SelectToken("price");
                    var shares = (int)s.SelectToken("size");

                    var ts = (double)s.SelectToken("sip_timestamp");

                    var trade = new RecentTradeModel
                    {
                        Price = price,
                        Shares = shares,
                        Timestamp = ts
                    };

                    trades.Add(trade);
                }

                return trades;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
