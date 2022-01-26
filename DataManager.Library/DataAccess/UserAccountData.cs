using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class UserAccountData
    {
        public UserPortfolioOverviewModel LoadPortfolioOverview(string id)
        {
            var sql = new SqlDataAccess();

            //TODO: Remove hardcode on user id
            var p = new { UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c" };

            try
            {
                var output = sql.LoadData<UserPortfolioOverviewModel, dynamic>("dbo.spUserAccount_GetPortfolioOverview", p, "StockPileData").First();
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateAccountBalance(UpdateUserAccountModel update)
        {
            var sql = new SqlDataAccess();
            var p = new { UserId = update.UserId, CashAmount = update.Amount };

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spUserAccount_UpdateAccountBalance", p);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        public decimal UpdateAfterSale(UpdateUserAccountModel update)
        {
            var sql = new SqlDataAccess();
            var p = new { UserId = update.UserId, RealizedProfitLoss = update.RealizedProfitLoss, SaleAmount = update.Amount};

            try
            {
                var output = sql.LoadData<decimal, dynamic>("dbo.spUserAccount_UpdateAfterSale", p, "StockPileData").First();
                return output;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
