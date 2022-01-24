using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class PortfolioData
    {
        public List<PortfolioStockDashboardModel> LoadPortfolio(string id)
        {
            var sql = new SqlDataAccess();

            //TODO: Remove hardcode of user id
            var p = new { UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c" };

            try
            {
                var output = sql.LoadData<PortfolioStockDashboardModel, dynamic>("dbo.spPortfolio_GetAllByUserId", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PortfolioStockDashboardModel LoadPortfolioStock(string id, string ticker)
        {
            var sql = new SqlDataAccess();

            //TODO: Remove hardcode of user id
            var p = new { UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c", Ticker = ticker };

            try
            {
                var output = sql.LoadData<PortfolioStockDashboardModel, dynamic>("dbo.spPortfolio_GetStockByTicker", p, "StockPileData").First();
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
