using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITradeRealizedPLEndpoint
    {
        Task PostProfitLoss(decimal realizedProfitLoss);
        Task<List<RealizedPLChartModel>> LoadHistory();
        Task<List<LeaderboardModel>> LoadRealizedPL(string id);
        Task<List<RealizedPLChartModel>> LoadHistoryByUserId(string id);
    }
}