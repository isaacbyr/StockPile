using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IStockDataEndpoint
    {
        Task<List<OhlcStockModel>> GetDashboardCharts(string ticker);
        Task<StockDashboardDataModel> GetStockDashboardData(string ticker);
        Task<List<StockDashboardDataModel>> GetMultipleStockDashboardData(string query);
    }
}