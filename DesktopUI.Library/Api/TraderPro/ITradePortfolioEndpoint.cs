using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITradePortfolioEndpoint
    {
        Task<PortfolioStockDashboardModel> GetPortfolioStock(string ticker);
        Task PostStock(PortfolioModel stock);
    }
}