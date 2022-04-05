using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess.Shared
{
    public class CommentsData
    {
        public List<CommentModel> GetCommentsByTicker(string ticker)
        {
            var sql = new SqlDataAccess();

            var p = new { Ticker = ticker };

            try
            {
                var output = sql.LoadData<CommentModel, dynamic>("dbo.spComments_LoadCommentsByTicker", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<CommentModel> GetAllComments()
        {
            var sql = new SqlDataAccess();

            var p = new { };

            try
            {
                var output = sql.LoadData<CommentModel, dynamic>("dbo.spComments_LoadAllComments", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TrendingModel GetTrendingStock()
        {
            var sql = new SqlDataAccess();

            var p = new { };

            try
            {
                var output = sql.LoadData<TrendingModel, dynamic>("dbo.spComments_LoadTrending", p, "StockPileData").First();
                return output;
            }
            catch (Exception ex)
            {
                return new TrendingModel { Ticker = "AAPL", Count = 5 };
            }
        }

        public void PostNewComment(NewCommentModel newComment)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spComments_PostNewComment", newComment);
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
