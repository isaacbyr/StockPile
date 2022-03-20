using DesktopUI.Library.Models;
using DesktopUI.Library.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IPortfolioEndpoint
    {
        Task<List<PortfolioStockDashboardModel>> LoadPortfolioStocks();
        Task<PortfolioStockDashboardModel> GetPortfolioStock(string ticker);
        Task UpdatePortfolioBuy(PortfolioModel stock);
        Task PostStock(PortfolioModel stock);
        Task<decimal> UpdatePortfolioSell(PortfolioModel stock);
        Task<decimal> UpdateAndDeletePortfolio(PortfolioModel stock);
        Task<List<TopHoldingsModel>> LoadTopHoldings();
    }
}