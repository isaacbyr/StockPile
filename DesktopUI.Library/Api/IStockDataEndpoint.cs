using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IStockDataEndpoint
    {
        Task<List<OhlcStockModel>> GetSpyIntra(string ticker);
        Task<List<StockDataModel>> GetSpyData(string ticker, string start, string end, string period);
        Task<List<StockDataModel>> GetDowData(string ticker);
    }
}