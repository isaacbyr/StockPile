using DesktopUI.Library.Models;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface ITransactionEndoint
    {
        Task PostTransaction(TransactionModel transaction);
    }
}