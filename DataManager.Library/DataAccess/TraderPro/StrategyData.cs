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
        public ResponseModel PostStrategy(StrategyModel strategy)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spStrategy_PostStrategy", strategy);
                sql.CommitTransaction();

                var response = new ResponseModel
                {
                    Header = "Success!",
                    Message = $"Successfully added new strategy {strategy.Name}"
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
