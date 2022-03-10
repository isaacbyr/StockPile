using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IUserAccountEndpoint
    {
        Task<decimal> LoadTradesAccountBalance();
        Task<decimal> LoadPortfolioAccountBalance();
        Task<UserPortfolioOverviewModel> GetPortfolioOverview();
        Task UpdatePortfolioAccountBalance(decimal cashAmount);
        Task UpdateTradesAccountBalance(decimal cashAmount);
        Task<decimal> UpdateAfterSale(decimal realizedProfitLoss, decimal cashAmount);
        Task PostNewUserAccount(UserAccountModel userAccount);
    }
}