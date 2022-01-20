using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IStockDataEndpoint
    {
        Task<List<StockDataModel>> GetSpyData(string ticker, string start, string end, string period);
    }
}