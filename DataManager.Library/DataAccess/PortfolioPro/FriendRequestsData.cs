using DataManager.Library.DataAccess.InternalAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.DataAccess
{
    public class FriendRequestsData
    {
        public List<FriendModel> LoadFriendRequests(string id)
        {
            var sql = new SqlDataAccess();

            var p = new { FolloweeId = id };

            try
            {
                var output = sql.LoadData<FriendModel, dynamic>("dbo.spFriendRequests_LoadRequestsById", p, "StockPileData");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteRequest(string followeeId, string followerId)
        {
            var sql = new SqlDataAccess();

            var p = new { FolloweeId = followeeId, FollowerId = followerId };

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spFriendRequests_DeleteRequest", p);
                sql.CommitTransaction();
            }
            catch (Exception e)
            {
                sql.RollbackTransaction();
                throw new Exception(e.Message);
            }
        }

        public void PostRequest(FriendRequestModel request)
        {
            var sql = new SqlDataAccess();

            try
            {
                sql.StartTransaction("StockPileData");
                sql.SaveDataInTransaction("dbo.spFriendRequests_NewRequest", request);
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
