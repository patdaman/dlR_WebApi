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

    public class CasePaymentController : ApiController
    {
        CasePaymentProcessor CasePaymentProc = new CasePaymentProcessor();

        // GET: api/CasePayment/
        // -- Returns a list of CasePayments ordered by Descending Post date

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the CasePayments. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <returns>   The bai CasePayments. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCasePayments()
        {
            try
            {
                return Request.CreateResponse<List<CaseFinancial>>(HttpStatusCode.OK, CasePaymentProc.GetCasePaymentList());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/CasePayment/5

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets CasePayments from a specific transaction </summary>
        ///
        /// <remarks>   Dtorres, 20160123. </remarks>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The bai file CasePayments. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCasePayment(int id)
        {
            try
            {
                return Request.CreateResponse<List<CasePaymentInfo>>(HttpStatusCode.OK, CasePaymentProc.GetCasePaymentsByTransaction(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        public HttpResponseMessage GetCasePayment(string CaseNumber)
        {
            try
            {
                return Request.CreateResponse<List<CaseFinancial>>(HttpStatusCode.OK, CasePaymentProc.GetCasePaymentByCaseNumber(CaseNumber));
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
        /// GET: api/CasePayment?fromDate=&toDate=

        public HttpResponseMessage GetCasePayment(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return Request.CreateResponse<List<CaseFinancial>>(HttpStatusCode.OK, CasePaymentProc.GetCasePaymentList(fromDate, toDate));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // PUT: api/CasePayment/5
        public HttpResponseMessage PutCasePayment(CaseFinancial cfUpdated)
        {
            try
            {
                CheckIfActionAuthorized(cfUpdated);
                CaseFinancial c = CasePaymentProc.UpdateCasePayment(cfUpdated);
                var response = Request.CreateResponse<CaseFinancial>(HttpStatusCode.OK, c);
                string uri = Url.Link("DefaultApi", new { id = c.ID });
                response.Headers.Location = new Uri(uri);

                return response;
                //return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }

        }

        private void CheckIfActionAuthorized(CaseFinancial theCase)
        {
            //case notes must be non-null/non-empty 
            if (theCase.Notes == null || theCase.Notes == "") throw new SignalException(SignalExceptionTypes.InvalidArguments_Error, "Empty Update Note");

            //check if user wants to edit billing class and if so, do they have permission
            bool isAuthorized = CheckOpenCloseStatusEditAuthorization(theCase);
            if (!isAuthorized)
            {
                throw new SignalException(SignalExceptionTypes.UpdateFailure_Error, "You are not authorized to edit Open/Closed Status.");
            }
            bool isZeroBalance = CheckRemainingBalance(theCase);
            if (!isZeroBalance)
            {
                throw new SignalException(SignalExceptionTypes.UpdateFailure_Error, "Cannot close unless AR Balance is 0.");
            }
        }
        private bool CheckOpenCloseStatusEditAuthorization(CaseFinancial updatedCase)
        {
            bool isAuthorizedToContinue = true;
            CaseFinancial originalCase = CasePaymentProc.GetCasePaymentByCaseNumber(updatedCase.CaseNumber).FirstOrDefault();
            if (updatedCase.Status != originalCase.Status) //then user want to make an edit to this field.
            {
                isAuthorizedToContinue = User.IsInRole("BillingSuite_Recon_Closer");
            }
            return isAuthorizedToContinue;
        }
        private bool CheckRemainingBalance(CaseFinancial updatedCase)
        {
            bool isAuthorizedToContinue = true;
            CaseFinancial originalCase = CasePaymentProc.GetCasePaymentByCaseNumber(updatedCase.CaseNumber).FirstOrDefault();
            if (updatedCase.Status != originalCase.Status && updatedCase.Status == "Closed" && updatedCase.RemainingBalance != 0) //then user want to make an edit to this field when Balance is not 0.
            {
                isAuthorizedToContinue = false;
            }
            return isAuthorizedToContinue;
        }
        // DELETE: api/CasePayment/5
        // -- Delete Disabled
        //public void Delete(int id)
        //{
        //}
    }
}
