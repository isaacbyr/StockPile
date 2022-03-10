using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api.TraderPro
{
    public interface ITradeTransactionEndpoint
    {
        Task PostTransaction(TransactionModel transaction);
    }
}