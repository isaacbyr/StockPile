using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IUserAccountEndpoint
    {
        Task<UserPortfolioOverviewModel> GetPortfolioOverview();
        Task UpdateAccountBalance(decimal cashAmount);
        Task<decimal> UpdateAfterSale(decimal realizedProfitLoss, decimal cashAmount);
        Task PostNewUserAccount(UserAccountModel userAccount);
    }
}