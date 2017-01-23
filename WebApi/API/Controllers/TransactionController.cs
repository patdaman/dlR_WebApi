using SignalBusinessLayer;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using SignalViewModel.BaiFile;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_Reconciliation")]
#endif
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A controller for handling transaction data. </summary>
    ///
    /// <remarks>   Pdelosreyes, 1/4/2016. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public class TransactionController : ApiController
    {
        TransactionProcessor transactionProc = new TransactionProcessor();
        BaiProcessor baiProc = new BaiProcessor();

        // GET: api/Transaction/
        // -- Returns a list of transactions ordered by Descending Post date

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the transactions. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <returns>   The bai transactions. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetTransactions()
        {
            try
            {
                return Request.CreateResponse<List<SignalViewModel.BaiFile.TransactionInfo>>(HttpStatusCode.OK, transactionProc.GetTransactionList());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/Transaction/5
        // -- Returns the data from a specific BAI file

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get cases associated with a transaction. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   Cases reconciled with transaction. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetTransaction(int id)
        {
            try
            {
                return Request.CreateResponse<TransactionData>(HttpStatusCode.OK, transactionProc.GetTransactionData(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/Transaction?fromDate={fromDate}&toDate={toDate}&dateType={dateType}
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Get transactions within date range. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <param name="fromDate"> from date. </param>
        /// <param name="toDate">   to date. </param>
        ///
        /// <returns>   The bai. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return Request.CreateResponse<List<SignalViewModel.BaiFile.TransactionInfo>>(HttpStatusCode.OK, transactionProc.GetTransactions(fromDate, toDate));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }


        // PUT: api/Transaction/5

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Puts a transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160212. </remarks>
        ///
        /// <param name="theTransaction">   the transaction. </param>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage PutTransaction(SignalViewModel.BaiFile.TransactionInfo theTransaction)
        {
            try
            {
                var updateTransaction = transactionProc.UpdateTransaction(theTransaction);
                var response = Request.CreateResponse<SignalViewModel.BaiFile.TransactionInfo>(HttpStatusCode.OK, updateTransaction);
                string uri = Url.Link("DefaultApi", new { id = updateTransaction.TransactionId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
            catch (Exception ex)
            {
                var sgnlEx = new SignalException(SignalExceptionTypes.RetrieveFailure_Error, "Error. See InnerException.", ex);
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, sgnlEx);
                return response;
            }
        }


        public HttpResponseMessage DeleteTransaction(int transactionId)
        {
            try
            {
                TransactionInfo tx = transactionProc.GetTransaction(transactionId);
                BaiFile bf = baiProc.GetBaiFile(tx.BaiFileId.Value);
                if (tx.Description != "SERVICE CHARGE" && tx.Description != "Deposit LOCKBOX # 0000010170" && bf.FileName.Split('.')[1] != ".csx")
                    throw new SignalException(SignalExceptionTypes.GeneralFailure_Error, "Only 'Service Charge' and 'Deposit Lockbox' from .csx uploaded file can be deleted. Reload grid to bring transaction " + transactionId + " back into view.");
                if (tx.CaseCount > 0)
                    throw new SignalException(SignalExceptionTypes.GeneralFailure_Error, "Cannot delete transaction with a case linked to it. Reload grid to bring transaction " + transactionId + " back into view.");
                if (!transactionProc.IsTransactionDuplicate(tx))
                    throw new SignalException(SignalExceptionTypes.GeneralFailure_Error, "Can only delete transaction if there is another with duplicate date/description/amount. Reload grid to bring transaction " + transactionId + " back into view.");

                transactionProc.DeleteTransaction(transactionId);
                var response = Request.CreateResponse<int>(HttpStatusCode.OK,transactionId);
                string uri = Url.Link("DefaultApi", new { id = transactionId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
            catch (Exception ex)
            {
                var sgnlEx = new SignalException(SignalExceptionTypes.RetrieveFailure_Error, "Error. See InnerException.", ex);
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, sgnlEx);
                return response;
            }
        }
        // DELETE: api/Bai/5
        // -- Delete Disabled
        //public void Delete(int id)
        //{
        //}
    }
}
