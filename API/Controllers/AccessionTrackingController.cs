using SignalBusinessLayer;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_CaseEditor, BillingSuite_BillingReporter, BillingSuite_DailyStatusReport, BillingSuite_Tracker")]
#endif
    public class AccessionTrackingController : ApiController
    {
        AccessionTrackingProcessor accProc = new AccessionTrackingProcessor();

        public HttpResponseMessage GetAccessionTracking()
        {
            try
            {
                return Request.CreateResponse<List<AccessionTracking>>(HttpStatusCode.OK, accProc.GetAccessionTracking());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        public HttpResponseMessage GetAccessionTracking(DateTime fromDate,
            DateTime toDate,
            string datetype
            )
        {
            try
            {
                List<AccessionTracking> returnedItem = accProc.GetAccessionTracking(fromDate, toDate, datetype);
                var response = Request.CreateResponse<List<AccessionTracking>>(HttpStatusCode.Accepted, returnedItem);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }

        }

        public HttpResponseMessage GetAccessionTracking(string caseListStr, char delim)
        {
            if (caseListStr == null || caseListStr == "")
            {
                var response = Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No case list specified");
                return response;
            }
            try
            {
                string[] caseList = caseListStr.Trim().Split(delim);
                var cases = accProc.GetAccessionTrackingCases(caseList.Distinct().ToArray());
                var response = Request.CreateResponse<List<AccessionTracking>>(HttpStatusCode.OK, cases);
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

        public HttpResponseMessage PutAccessionTracking(AccessionTracking theCase)
        {
            try
            {
                var updateCase = accProc.RefreshAccessionTracking(theCase.CaseNumber);
                var response = Request.CreateResponse<AccessionTracking>(HttpStatusCode.OK, updateCase);
                string uri = Url.Link("DefaultApi", new { id = updateCase.CaseNumber });
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
    }
}