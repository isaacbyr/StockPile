using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess.TraderPro
{
    public class TradeTransactionData
    {
        public void PostTransaction(TransactionModel transaction)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spTradeTransaction_PostTransaction", transaction);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        public List<TransactionModel> LoadCurrentTransactions(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<TransactionModel, dynamic>("dbo.spTradeTransaction_LoadCurrent", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<TransactionModel> LoadTransactions(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<TransactionModel, dynamic>("dbo.spTradeTransaction_LoadById", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<TransactionChartData> LoadChartData(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<TransactionChartData, dynamic>("dbo.spTradeTransaction_LoadChartData", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SocialDashboardDataModel> LoadDashboardById(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { Id = id };

            try
            {
                var output = sql.LoadData<SocialDashboardDataModel, dynamic>("dbo.spTradeTransaction_LoadDashboardById", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
