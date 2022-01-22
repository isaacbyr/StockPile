using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IPortfolioEndpoint
    {
        Task<List<PortfolioStockDashboardModel>> LoadPortfolioStocks();
    }
}