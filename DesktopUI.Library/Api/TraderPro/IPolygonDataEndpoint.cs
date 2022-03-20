using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface IPolygonDataEndpoint
    {
        Task<(List<double>, List<double>, List<double>, List<double>, List<DateTime>)> LoadTradeData(string ticker, int interval, string startTimestamp, string endTimestamp);
        Task<List<RecentTradeModel>> LoadRecentTrades(string ticker, double timestamp);
        Task<List<double>> LoadMAData(string ticker, int interval, string startTimestamp, string endTimestamp, int lastResults);
        Task<(List<double>, List<double>, List<double>, List<double>, List<DateTime>)> LoadChartData(string ticker, int interval, string startTimestamp, string endTimestamp);
    }
}