using SignalBusinessLayer;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_CaseEditor, BillingSuite_BillingReporter, BillingSuite_DailyStatusReport")]
#endif
    public class BillingStatusCasesController : ApiController
    {
        BillingStatusCasesProcessor processor = new BillingStatusCasesProcessor();

        private const string Filter_Unbilled = "unbilled";

        /// <summary>
        /// Pulls data from the given date range.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="dateType">Specifies the date type. Can be "orderdate" or "completeddate" </param>
        /// <returns></returns>
        public HttpResponseMessage GetBillingStatusCases(
            [FromUri] string fromDate,
            [FromUri] string toDate,
            [FromUri] string dateType)
        {
            //var sgnlE1x = new SignalException(SignalExceptionTypes.RetrieveFailure_Error, "Error. See InnerException.", null);
            //throw sgnlE1x;
            try
            {
                DateTime? fromdate = ConvertToNullableDate(fromDate);
                DateTime? todate = ConvertToNullableDate(toDate);

                SignalDateType dateType_;
                switch (dateType.ToLower())
                {
                    case "orderdate":
                        dateType_ = SignalDateType.OrderDate;
                        break;
                    case "completeddate":
                        dateType_ = SignalDateType.CompletedDate;
                        break;
                    default:
                        throw new
                            SignalException(SignalExceptionTypes.InvalidArguments_Error, "Unknown date type encountered: " + dateType);
                }
                var cases = processor.GetBillingStatusCases(fromdate, todate, dateType_);
                var response = Request.CreateResponse<List<BillingStatusCase>>(HttpStatusCode.OK, cases);
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
        /// <summary>   Gets billing status cases by case list
        ///              </summary>
        ///
        /// <remarks>   Rphilavanh, 20151028. </remarks>
        ///
        /// <exception cref="SignalException">  Thrown when a Signal error condition occurs. </exception>
        ///
        /// <param name="caseListStr"> List of cases. </param>
        ///  <param name="delim"> List of cases. </param>
        ///
        /// <returns>   The billing status cases. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetBillingStatusCases(string caseListStr, char delim)
        {
            if (caseListStr == null || caseListStr == "")
            {
                var response = Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No case list specified");
                return response;
            }
            try
            {
                string[] caseList = caseListStr.Trim().Split(delim);
                var cases = processor.GetBillingStatusCases(caseList.Distinct().ToArray());
                var response = Request.CreateResponse<List<BillingStatusCase>>(HttpStatusCode.OK, cases);
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
        /// <summary>   Gets unbilled cases. </summary>
        ///
        /// <remarks>   Dtorres, 20150925. </remarks>
        ///
        /// <exception cref="SignalException">  Thrown when a Signal error condition occurs. </exception>
        ///
        /// <param name="fromDate">         . </param>
        /// <param name="toDate">           . </param>
        ///  <param name="dateType">           . </param>
        /// <param name="billingAggregate"> The billing aggregate. </param>
        ///
        /// <returns>   The unbilled. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetUnbilled([FromUri] string fromDate,
            [FromUri] string toDate,
            [FromUri] string dateType,
            [FromUri] string billingAggregate)
        {

            DateTime? FromDate = ConvertToNullableDate(fromDate);
            DateTime? ToDate = ConvertToNullableDate(toDate);
            if (billingAggregate == null) throw new SignalException(SignalExceptionTypes.InvalidArguments_Error, "BillingAggregate argument was null.");
            try
            {
                SignalDateType dateType_;
                switch (dateType.ToLower())
                {
                    case "orderdate":
                        dateType_ = SignalDateType.OrderDate;
                        break;
                    case "completeddate":
                        dateType_ = SignalDateType.CompletedDate;
                        break;
                    default:
                        throw new
                            SignalException(SignalExceptionTypes.InvalidArguments_Error, "Unknown date type encountered: " + dateType);
                }
                var cases = processor.GetUnbilledCases(FromDate, ToDate, billingAggregate, dateType_);
                var response = Request.CreateResponse<List<BillingStatusCase>>(HttpStatusCode.OK, cases);
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

        private DateTime? ConvertToNullableDate(string dateString)
        {
            DateTime? date = dateString == null ? null : new DateTime?(Convert.ToDateTime(dateString));
            return date;
        }

        public HttpResponseMessage GetBillingStatusCases(int id)
        {
            try
            {
                var theCase = processor.GetBillingStatusCase(id);
                var response = Request.CreateResponse<BillingStatusCase>(HttpStatusCode.OK, theCase);
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

        public HttpResponseMessage GetBillingStatusCases(string filter)
        {
            try
            {
                switch (filter)
                {
                    case Filter_Unbilled:
                        var cases = processor.GetUnbilledCases();
                        var response = Request.CreateResponse<List<BillingStatusCase>>(HttpStatusCode.OK, cases);
                        return response;
                    default:
                        throw new SignalException(SignalExceptionTypes.InvalidArguments_Error, "Unknown filter");
                }

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


        /// <summary>
        /// Returns the latest case in the database.
        /// </summary>
        /// <param name="latest"></param>
        /// <returns></returns>
        public HttpResponseMessage GetLatestCase(string latest)
        {
            try
            {
                BillingStatusCase thecase = processor.GetLatestCase();
                var response = Request.CreateResponse<BillingStatusCase>(HttpStatusCode.OK, thecase);
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

        public HttpResponseMessage PutBillingStatusCase(BillingStatusCase theCase)
        {
            try
            {
                // THROWS an exception if the action is not authorized
                CheckIfActionAuthorized(theCase);
                var updateCase = processor.UpdateBillingStatusCase(theCase);
                var response = Request.CreateResponse<BillingStatusCase>(HttpStatusCode.OK, updateCase);
                string uri = Url.Link("DefaultApi", new { id = updateCase.CaseId });
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

        private void CheckIfActionAuthorized(BillingStatusCase theCase)
        {
            //case notes must be non-null/non-empty 
            if (theCase.NewNotes == null || theCase.NewNotes == "") throw new SignalException(SignalExceptionTypes.InvalidArguments_Error, "Empty Update Note");

            //check if user wants to edit billing class and if so, do they have permission
            bool isAuthorized = CheckBillingCategoryEditAuthorization(theCase);
            if( ! isAuthorized )
            {
                throw new SignalException(SignalExceptionTypes.UpdateFailure_Error, "You are not authorized to edit Billing Classification.");
            }
            bool isCloseAuthorized = CheckBillableStatusEditAuthorization(theCase);
            if (!isCloseAuthorized)
            {
                throw new SignalException(SignalExceptionTypes.UpdateFailure_Error, "You are not authorized to edit Bill Status.");
            }
        }

        private bool CheckBillableStatusEditAuthorization(BillingStatusCase theCase)
        {
            bool isAuthorizedToContinue = true;
            BillingStatusCase originalCase = processor.GetBillingStatusCase(theCase.CaseId);
            if (theCase.BillableStatus != originalCase.BillableStatus) //then user want to make an edit to this field.
            {
#if DEBUG
                isAuthorizedToContinue = true;
#else
                isAuthorizedToContinue = User.IsInRole("BillingSuite_Billing_Admin");                
#endif
            }
            return isAuthorizedToContinue;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Check billing category edit authorization. </summary>
        ///
        /// <remarks>   Dtorres, 20160219. </remarks>
        ///
        /// <param name="updatedCase">  The updated case. </param>
        ///
        /// <returns>   true user is allowed to continue or make edits, false otherwise </returns>
        ///-------------------------------------------------------------------------------------------------

        private bool CheckBillingCategoryEditAuthorization(BillingStatusCase updatedCase)
        {
            bool isAuthorizedToContinue = true;
            BillingStatusCase originalCase = processor.GetBillingStatusCase(updatedCase.CaseId);
            if( updatedCase.BillingClassification != originalCase.BillingClassification ) //then user want to make an edit to this field.
            {
#if DEBUG
                isAuthorizedToContinue = true;
#else
                isAuthorizedToContinue = User.IsInRole("BillingSuite_Billing_Admin");                
#endif
            }
            return isAuthorizedToContinue;
        }
    }
}