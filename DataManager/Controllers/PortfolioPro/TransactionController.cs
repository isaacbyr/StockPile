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

            transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            transactionData.PostTransaction(transaction);
        }

        [HttpGet]
        public List<TransactionModel> LoadTransactions()
        {

            var transactionData = new TransactionData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return transactionData.LoadTransactions(id);
        }

        [HttpGet]
        [Route("chart")]
        public List<TransactionChartData> LoadChartData()
        {
            var transactionData = new TransactionData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return transactionData.LoadChartData(id);
        }

        [HttpGet]
        [Route("{id}")]
        public List<SocialDashboardDataModel> LoadById(string id)
        {
            var transactionData = new TransactionData();

            return transactionData.LoadDashboardById(id);
        }
    }
}
