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
    public class AccessionErrorsController : ApiController
    {
        AccessionTrackingProcessor processor = new AccessionTrackingProcessor();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets accession errors. </summary>
        ///
        /// <remarks>   Rphilavanh, 20160222. </remarks>
        ///
        /// <param name="caseno">   The caseno. </param>
        /// <param name="getAll">   true to get all. </param>
        ///
        /// <returns>   The accession errors. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetAccessionErrors(string caseno, bool getAll)
        {
            try
            {
                var err = processor.GetXifinErrorsFromCase(caseno, getAll);
                var response = Request.CreateResponse<List<XifinBillingError>>(HttpStatusCode.OK, err);
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