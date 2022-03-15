using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class WatchlistData
    {
        public List<WatchlistModel> LoadWatchlistStocksById(string id)
        {
            var sql = new SqlDataAccess();

            var p = new {UserId = id };

            try
            {
                var output = sql.LoadData<WatchlistModel, dynamic>("dbo.spWatchlist_LoadAllById", p, "StockPileData");
                return output;
            }
            catch
            {
                throw new Exception();
            }
        }

        public void PostWatchlistStock(WatchlistModel stock)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spWatchlist_InsertWatchlistStock", stock);
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
