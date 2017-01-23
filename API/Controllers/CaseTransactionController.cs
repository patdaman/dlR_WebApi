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
using System.Diagnostics;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_Reconciliation")]
#endif
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A controller for handling cases reconciled against transactions. </summary>
    ///
    /// <remarks>   Pdelosreyes, 1/4/2016. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class CaseTransactionController : ApiController
    {
        CaseTransactionProcessor CaseTransactionProc = new CaseTransactionProcessor();

        // GET: api/CaseTransaction/
        // -- Returns a list of CaseTransactions ordered by Descending Post date

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the CaseTransactions. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <returns>   The bai CaseTransactions. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransactions()
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK, CaseTransactionProc.GetCaseTransactionList());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/CaseTransaction/5

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets CaseTransactions from a specific transaction </summary>
        ///
        /// <remarks>   Dtorres, 20160123. </remarks>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The bai file CaseTransactions. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransaction(int id)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK, CaseTransactionProc.GetCasesByTransaction(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case transaction from Case Number. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160214. </remarks>
        ///
        /// <param name="CaseNumber">   The case number. </param>
        ///
        /// <returns>   The case transaction. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransaction(string CaseNumber)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK,
                    CaseTransactionProc.GetCaseTransactionList(
                        CaseNumber,
                        0,
                        "",
                        "",
                        null,
                        null));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160219. </remarks>
        ///
        /// <param name="TransactionId">    Identifier for the transaction. </param>
        /// <param name="CaseNumber">       The case number. </param>
        ///
        /// <returns>   The case transaction. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransaction(int TransactionId, string CaseNumber)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK,
                    CaseTransactionProc.GetCaseTransactionList(
                        CaseNumber,
                        TransactionId,
                        "",
                        "",
                        null,
                        null));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160302. </remarks>
        ///
        /// <param name="NewTransId">       Identifier for the new transaction. </param>
        /// <param name="CaseNumber">       The case number. </param>
        /// <param name="TransactionId">    Identifier for the transaction. </param>
        ///
        /// <returns>   The case transaction. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransaction(int NewTransId, string CaseNumber, int? TransactionId)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK,
                    CaseTransactionProc.GetCaseTransactionList(
                        CaseNumber,
                        0,
                        "",
                        "",
                        null,
                        null,
                        NewTransId));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets Cases within date range </summary>
        ///
        /// <remarks>   Dtorres, 20160123. </remarks>
        ///
        /// <param name="fromDate"> from date. </param>
        /// <param name="toDate">   to date. </param>
        ///
        /// <returns>   The bai. </returns>
        ///-------------------------------------------------------------------------------------------------
        /// GET: api/CaseTransaction?fromDate=&toDate=

        public HttpResponseMessage GetCaseTransaction(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK,
                    CaseTransactionProc.GetCaseTransactionList(
                        "",
                        0,
                        "",
                        "",
                        fromDate,
                        toDate));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160216. </remarks>
        ///
        /// <param name="CaseNumber">       The case number. </param>
        /// <param name="TransactionId">    Identifier for the transaction. </param>
        /// <param name="Payor">            The payor. </param>
        /// <param name="PayorGroup">       Group the payor belongs to. </param>
        /// <param name="fromDate">         from date. </param>
        /// <param name="toDate">           to date. </param>
        /// <param name="format">           Describes the format to use. </param>
        ///
        /// <returns>   The case transaction. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseTransaction(string CaseNumber, int? TransactionId, string Payor, string PayorGroup, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK, 
                    CaseTransactionProc.GetCaseTransactionList(
                        CaseNumber, 
                        TransactionId, 
                        Payor, 
                        PayorGroup, 
                        fromDate, 
                        toDate));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        public HttpResponseMessage GetCaseTransaction(string CaseNumber, int? TransactionId, string Payor, string PayorGroup, DateTime? fromDate, DateTime? toDate, int? newTransId)
        {
            try
            {
                return Request.CreateResponse<List<CaseTransactionInfo>>(HttpStatusCode.OK,
                    CaseTransactionProc.GetCaseTransactionList(
                        CaseNumber,
                        TransactionId,
                        Payor,
                        PayorGroup,
                        fromDate,
                        toDate,
                        newTransId));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // PUT: api/CaseTransaction/5

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Puts case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160216. </remarks>
        ///
        /// <param name="theCaseTransaction">   the case transaction. </param>
        ///
        /// <returns>   A HttpResponseMessage. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage PutCaseTransaction(CaseTransactionInfo theCaseTransaction)
        {
            try
            {
                var updateCaseTransaction = CaseTransactionProc.UpdateCaseTransaction(theCaseTransaction);
                var response = Request.CreateResponse<CaseTransactionInfo>(HttpStatusCode.OK, updateCaseTransaction);
                string uri = Url.Link("DefaultApi", new { id = updateCaseTransaction.TransactionId });
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Posts a case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160304. </remarks>
        ///
        /// <param name="theCaseTransaction">   the case transaction. </param>
        ///
        /// <returns>   A HttpResponseMessage. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage PostCaseTransaction(CaseTransactionInfo theCaseTransaction)
        {
            try
            {
                var updateCaseTransaction = CaseTransactionProc.UpdateCaseTransaction(theCaseTransaction);
                var response = Request.CreateResponse<CaseTransactionInfo>(HttpStatusCode.OK, updateCaseTransaction);
                string uri = Url.Link("DefaultApi", new { id = updateCaseTransaction.TransactionId });
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

        // DELETE: api/CaseTransaction/5

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Deletes the case transaction. </summary>
        ///
        /// <remarks>   Pdelosreyes, 20160219. </remarks>
        ///
        /// <param name="transactionId">    Identifier for the transaction. </param>
        /// <param name="caseNumber">       The case number. </param>
        ///
        /// <returns>   A HttpResponseMessage. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage DeleteCaseTransaction(int transactionId, string caseNumber)
        {
            Debug.Assert(caseNumber != null, "DeleteCaseTransaction: CaseNumber argument was null.");
            if (caseNumber == null)
                throw new SignalException(SignalExceptionTypes.UpdateFailure_Error, "DeleteCaseTransaction: CaseNumber argument was null.");

            try
            {
                var deleteCaseTransaction = CaseTransactionProc.DeleteCaseTransaction(transactionId, caseNumber);
                var response = Request.CreateResponse<Boolean>(HttpStatusCode.OK, deleteCaseTransaction);
                //string uri = Url.Link("DefaultApi", new { id = transactionId });
                //response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
            catch (Exception ex)
            {
                var sgnlEx = new SignalException(SignalExceptionTypes.UpdateFailure_Error, "Error. See InnerException.", ex);
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, sgnlEx);
                return response;
            }
        }
    }
}
