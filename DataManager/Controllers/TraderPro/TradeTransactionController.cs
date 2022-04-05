using DataManager.Library.DataAccess.TraderPro;
using DataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers.TraderPro
{
    [RoutePrefix("api/tradetransaction")]
    public class TradeTransactionController : ApiController
    {
        [HttpGet]
        public List<TransactionModel> LoadTransactions()
        {

            var transactionData = new TradeTransactionData();

            string id = RequestContext.Principal.Identity.GetUserId();
            //string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return transactionData.LoadTransactions(id);
        }

        [HttpGet]
        [Route("today")]
        public List<TransactionModel> LoadCurrentTransactions()
        {
            var transactionData = new TradeTransactionData();

            string id = RequestContext.Principal.Identity.GetUserId();
            //string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return transactionData.LoadCurrentTransactions(id);
        }

        [HttpGet]
        [Route("chart")]
        public List<TransactionChartData> LoadChartData()
        {
            var transactionData = new TradeTransactionData();

            string id = RequestContext.Principal.Identity.GetUserId();
            //string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return transactionData.LoadChartData(id);
        }

        [HttpPost]
        public void PostTransaction(TransactionModel transaction)
        {
            var transactionData = new TradeTransactionData();

            transaction.UserId = RequestContext.Principal.Identity.GetUserId();
            //transaction.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            transactionData.PostTransaction(transaction);
        }

        [HttpGet]
        [Route("{id}")]
        public List<SocialDashboardDataModel> LoadById(string id)
        {
            var transactionData = new TradeTransactionData();

            return transactionData.LoadDashboardById(id);
        }
    }
}
