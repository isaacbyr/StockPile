using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers
{
    [RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        [HttpPost]
        public void PostTransaction(TransactionModel transaction)
        {
            var transactionData = new TransactionData();

            //TODO: Change hardcode on user id
            transaction.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            transactionData.PostTransaction(transaction);
        }

        [HttpGet]
        public List<TransactionModel> LoadTransactions()
        {

            var transactionData = new TransactionData();

            //TODO: Change hardcode on user id
            string id = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            return transactionData.LoadTransactions(id);
        }

        [HttpGet]
        [Route("chart")]
        public List<TransactionChartData> LoadChartData()
        {
            var transactionData = new TransactionData();

            //TODO: Change hardcode on user id
            string id = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            return transactionData.LoadChartData(id);
        }
    }
}
