using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IStockDataEndpoint
    {
        Task<StockDashboardDataModel> GetStockDashboardData(string ticker);
        Task<List<StockDashboardDataModel>> GetMultipleStockDashboardData(string query);
        Task<List<StockDashboardDataModel>> GetDailyGainersOrLosers(string query);
        Task<(List<OhlcStockModel>, string, string)> GetDashboardCharts(string ticker, string range = "3mo", string interval = "1d");
        Task<CompanyOverviewModel> GetCompanyOverview(string ticker);
        Task<(List<OhlcStockModel>, string, string)> GetMAChartData(string ticker, string range, string interval, int lastResult);
        Task<List<StockCloseModel>> GetCloseData(string ticker, string range, string interval);

    }
}