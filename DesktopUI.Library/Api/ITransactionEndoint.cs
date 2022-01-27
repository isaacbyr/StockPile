using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface ITransactionEndoint
    {
        Task PostTransaction(TransactionModel transaction);
        Task<List<TransactionModel>> LoadTransactions();
        Task<List<TransactionChartData>> LoadChartData();
    }
}