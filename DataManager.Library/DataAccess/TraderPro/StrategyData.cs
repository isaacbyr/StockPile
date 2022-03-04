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

            var postedStrategy = new { UserId = strategy.UserId, Name = strategy.Name, 
                                        MA1 = strategy.MA1, MA2 = strategy.MA2, Indicator = strategy.Indicator,
                                        Interval = strategy.Interval, Range = strategy.Range};
            var p = new { UserId = strategy.UserId };
            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spStrategy_PostStrategy", postedStrategy);
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

        public ResponseModel PostStrategyStock(StrategyItemModel strategyStock)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spStrategyItem_Post", strategyStock);
                sql.CommitTransaction();

                var response = new ResponseModel
                {
                    Header = "Success!",
                    Message = $"Successfully added New strategy"
                };
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseModel
                {
                    Header = "Error",
                    Message = $"{ex.Message}"
                };
                return response;
            }
        }

        public List<StrategyModel> LoadStrategies(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { UserId = id };

            try
            {
                var output = sql.LoadData<StrategyModel, dynamic>("dbo.spStrategy_GetAllStrategies", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
      }
}
