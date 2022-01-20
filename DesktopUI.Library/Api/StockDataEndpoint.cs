using DesktopUI.Library.Models;
using Newtonsoft.Json;
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

        public async Task<List<StockDataModel>> GetSpyData(string ticker, string start, string end, string period)
        {
            var p = Period.Daily;
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            var hist = await Yahoo.GetHistoricalAsync(ticker, startDate, endDate, p);

            List<StockDataModel> models = new List<StockDataModel>();
            foreach (var r in hist)
            {
                models.Add(new StockDataModel
                {
                    Ticker = ticker,
                    Date = r.DateTime.ToString("yyyy-MM-dd"),
                    Open = (double) r.Open,
                    High = (double) r.High,
                    Low = (double) r.Low,
                    Close = (double) r.Close,
                    AdjustedClose = r.AdjustedClose,
                    Volume = r.Volume
                }) ;
            }
            return models;
        }
    }
}
