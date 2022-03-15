using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class TransactionData
    {
        public void PostTransaction(TransactionModel transaction)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spTransaction_PostTransaction", transaction);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        public List<TransactionModel> LoadTransactions(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<TransactionModel, dynamic>("dbo.spTransaction_LoadById", p, "StockPileData");
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
                var output = sql.LoadData<TransactionChartData, dynamic>("dbo.spTransaction_LoadChartData", p, "StockPileData");
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
                var output = sql.LoadData<SocialDashboardDataModel, dynamic>("dbo.spTransaction_LoadDashboardById", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
