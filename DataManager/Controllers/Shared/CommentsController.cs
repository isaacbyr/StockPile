using DataManager.Library.DataAccess.Shared;
using DataManager.Library.Models.Shared;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers.Shared
{
    [RoutePrefix("api/comments")]
    public class CommentsController : ApiController
    {

        [HttpGet]
        [Route("{ticker}")]
        public List<CommentModel> GetCommentByTicker(string ticker)
        {
            var commentsData = new CommentsData();

            return commentsData.GetCommentsByTicker(ticker);
        }

        [HttpGet]
        public List<CommentModel> GetComments()
        {
            var commentsData = new CommentsData();

            return commentsData.GetAllComments();
        }

        [HttpGet]
        [Route("trending")]
        public TrendingModel GetTrendingStock()
        {
            var commentsData = new CommentsData();

            return commentsData.GetTrendingStock();
        }

        [HttpPost]
        public void PostNewComment(NewCommentModel comment)
        {
            var commentsData = new CommentsData();

            // TODO: REPLACE HARDCODE
            //comment.UserId = RequestContext.Principal.Identity.GetUserId();
            comment.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            commentsData.PostNewComment(comment);
        }
    }
}
