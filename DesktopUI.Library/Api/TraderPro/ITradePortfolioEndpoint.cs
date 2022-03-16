using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITradePortfolioEndpoint
    {
        Task<PortfolioStockDashboardModel> GetPortfolioStock(string ticker);
        Task PostStock(PortfolioModel stock);
        Task UpdatePortfolioBuy(PortfolioModel stock);
        Task<decimal> UpdateAndDeletePortfolio(PortfolioModel stock);
        Task<decimal> UpdatePortfolioSell(PortfolioModel stock);
        Task<List<PortfolioStockDashboardModel>> LoadPortfolioStocks();
    }
}