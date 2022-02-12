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
        public async Task<(List<double>, List<double>, List<double>, List<double>)> LoadTradeData(string ticker, string timestamp)
        {
            string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";


            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.polygon.io/");

            var response = await httpClient.GetAsync($"v2/aggs/ticker/{ticker}/range/1/minute/{timestamp}/{timestamp}?adjusted=true&sort=asc&limit=50000&apiKey={API_KEY}");

            if(response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var data = (JObject)JsonConvert.DeserializeObject(responseBody);

                var opens = new List<double>();
                var highs = new List<double>();
                var closes = new List<double>();
                var lows = new List<double>();

                int index = 0;
                int startIndex = 0;
                int endIndex = 0;
                foreach (var r in data.SelectToken("results"))
                {
                    var open = (double)r.SelectToken("o");
                    var high = (double)r.SelectToken("h");
                    var low = (double)r.SelectToken("l");
                    var close = (double)r.SelectToken("c");
                    var stringTs = (string)r.SelectToken("t");

                    var dt = ConvertTimestampToDate(stringTs, 1);

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


                return (opens, highs, lows, closes);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        private DateTime ConvertTimestampToDate(string stringTs, int interval)
        {

            int ts;
            int.TryParse(stringTs.Substring(0, 10), out ts);
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();

            return date;

        }

        //public async Task LoadTradeData(string ticker, string timestamp)
        //{
        //    // 630 timestamp
        //    //1644417004917261811

        //    string API_KEY = "g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1";

        //    var httpClient = new HttpClient();
        //    httpClient.BaseAddress = new Uri("https://api.polygon.io/");

        //    var response = await httpClient.GetAsync($"vX/trades/AAPL?timestamp.gt=1644417004917261811&order=asc&limit=30000&sort=timestamp&apiKey={API_KEY}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        var data = (JObject)JsonConvert.DeserializeObject(responseBody);

        //        var trades = new List<PaperTradeModel>();

        //        foreach(var r in data.SelectToken("results"))
        //        {
        //            var price = (double)r.SelectToken("price");
        //            var stringTs = (string)r.SelectToken("participant_timestamp");

        //            var dt = ConvertTimestampToDateAndRound(stringTs, 1);

        //            var trade = new PaperTradeModel
        //            {
        //                Date = dt,
        //                Price = price
        //            };

        //            trades.Add(trade);
        //        }

        //        var groupedTrades = new List<List<double>>();
        //        var tempTrades = new List<double>();
        //        var index = 0;

        //        var testDate = trades[0].Date;
        //        foreach(var t in trades)
        //        {
        //            if(t.Date == testDate)
        //            {
        //                tempTrades.Add(t.Price);
        //            }
        //            else
        //            {
        //                testDate = trades[index - 1].Date;
        //                groupedTrades.Add(tempTrades);
        //                tempTrades.Clear();
        //            }
        //            index++;
        //        }

        //    }

    }
}
