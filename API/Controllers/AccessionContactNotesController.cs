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
    public class AccessionContactNotesController : ApiController
    {
        AccessionTrackingProcessor processor = new AccessionTrackingProcessor();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets accession contact notes. </summary>
        ///
        /// <remarks>   Rphilavanh, 20160219. </remarks>
        ///
        /// <param name="caseno">   The caseno. </param>
        ///
        /// <returns>   The accession contact notes. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetAccessionContactNotes(string caseno)
        {
            try
            {
                var notes = processor.GetAccessionContactInfoFromCase(caseno);
                var response = Request.CreateResponse<List<XifinAccessionContact>>(HttpStatusCode.OK, notes);
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