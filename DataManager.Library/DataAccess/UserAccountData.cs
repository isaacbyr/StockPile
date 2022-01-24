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
    }
}
