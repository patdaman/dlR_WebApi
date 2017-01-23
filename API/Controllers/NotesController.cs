using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using SignalBusinessLayer;
using SignalViewModel;
using System.Net;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_CaseEditor")]
#endif
    public class NotesController : ApiController
    {
        NotesProcessor processor = new NotesProcessor();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the notes. </summary>
        ///
        /// <remarks>   Rphilavanh, 20151201. </remarks>
        ///
        /// <param name="caseno">   The caseno. </param>
        /// <param name="noteType"> Type of the note. </param>
        ///
        /// <returns>   The notes. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetNotes(string caseno, string noteType)
        {
            try
            {        
                var notes = processor.GetNotes(caseno, noteType);
                var response = Request.CreateResponse<List<Note>>(HttpStatusCode.OK, notes);
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