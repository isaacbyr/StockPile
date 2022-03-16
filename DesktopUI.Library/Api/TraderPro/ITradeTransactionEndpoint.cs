using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITradeTransactionEndpoint
    {
        Task PostTransaction(TransactionModel transaction);
        Task<List<SocialDashboardDataModel>> LoadTransactionsById(string id);
    }
}