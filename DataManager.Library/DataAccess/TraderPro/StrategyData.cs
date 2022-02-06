using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using DataManager.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess.TraderPro
{
    public class StrategyData
    {
        public int PostStrategy(StrategyModel strategy)
        {
            var sql = new SqlDataAccess();

            var p = new { };
            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spStrategy_PostStrategy", strategy);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }

            try
            {
                var output = sql.LoadData<int, dynamic>("dbo.spStrategy_GetStrategyId", p, "StockPileData").FirstOrDefault();
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ResponseModel PostStrategyStock(StrategyStockModel strategyStock)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spStrategy_PostStrategyStock", strategyStock);
                sql.CommitTransaction();

                var response = new ResponseModel
                {
                    Header = "Success!",
                    Message = $"Successfully added New strategy"
                };
                return response;
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                var response = new ResponseModel
                {
                    Header = "Error",
                    Message = $"{e.Message}"
                };
                return response;
            }
        }
    }
}
