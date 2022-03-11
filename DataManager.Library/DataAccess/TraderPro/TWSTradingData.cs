using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess.TraderPro
{
    public class TWSTradingData
    {

        public void PostTWSStrategy(TWSTradeModel trade)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spTWSTradingStrategies_PostStrategy", trade);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        
        public List<TWSTradeModel> LoadAllStrategies(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<TWSTradeModel, dynamic>("dbo.spTWSTradingStrategies_LoadAllStrategies", p, "StockPileData");
                return output;
            }
            catch
            {
                return null;
            }
        }
    }
}
