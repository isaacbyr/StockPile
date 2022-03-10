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
        [HttpPost]
        public void PostTransaction(TransactionModel transaction)
        {
            var transactionData = new TradeTransactionData();

            //TODO: REMOVE HARDCODE USER ID
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();
            transaction.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            transactionData.PostTransaction(transaction);
        }
    }
}
