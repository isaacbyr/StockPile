using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess.TraderPro
{
    public class TradesPortfolioData
    {
        public PortfolioStockDashboardModel LoadPortfolioStock(string id, string ticker)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id, Ticker = ticker };

            try
            {
                var output = sql.LoadData<PortfolioStockDashboardModel, dynamic>("dbo.spTradesPortfolio_GetStockByTicker", p, "StockPileData").First();
                return output;
            }
            catch
            {
                return null;
            }
        }

        public void UpdatePortfolioBuy(PortfolioModel stock)
        {

            var sql = new SqlDataAccess();

            var p = new { UserId = stock.UserId, Price = stock.Price, Shares = stock.Shares, Ticker = stock.Ticker };

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spTradesPortfolio_UpdatePortfolioBuy", p);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        public void PostStockToPortfolio(PortfolioModel stock)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = stock.UserId, AveragePrice = stock.Price, Shares = stock.Shares, Ticker = stock.Ticker };

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spTradesPortfolio_PostStock", p);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }


    }
}
