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
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_PayorEditor, BillingSuite_CaseEditor, BillingSuite_BillingReporter, BillingSuite_DailyStatusReport ")]
#endif
    public class PayorsController : ApiController
    {
        PayorsProcessor payorProc = new PayorsProcessor();



        //function name starting with Get without id maps to /api/Payors

        public HttpResponseMessage GetPayors()
        {            
            try
            {
                
                return Request.CreateResponse<List<Payor>>(HttpStatusCode.OK, payorProc.GetPayors());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }


        //function name starting with Get with single parameter called id maps to /api/Payors/{id}
        [ResponseType(typeof(Payor))]
        public HttpResponseMessage GetPayor(int id)
        {
            try
            {
                return Request.CreateResponse<Payor>(HttpStatusCode.OK, payorProc.GetPayors(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }

        }

        //function name starting with Get with single parameter called e.g. category maps to /api/Payors/?category=category
        [ResponseType(typeof(Payor))]
        public HttpResponseMessage GetPayor(string PayorCode)
        {
            try
            {
                return Request.CreateResponse<Payor>(HttpStatusCode.OK, payorProc.GetPayorByPayorCode(PayorCode));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        //PUT is for updates
        //function name starting with Put
        public HttpResponseMessage PutPayor(Payor thePayor)
        {
            try
            {
                Payor ret = payorProc.UpdatePayor(thePayor);
                var response = Request.CreateResponse<Payor>(HttpStatusCode.OK, ret);
                string uri = Url.Link("DefaultApi", new { id = ret.PayorId });
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

        //POST is for inserts. On success send uri of new item
        public HttpResponseMessage PostPayor(Payor newPayor)
        {
            try
            {
                Payor ret = payorProc.CreatePayor(newPayor);
                var response = Request.CreateResponse<Payor>(HttpStatusCode.Created, ret);
                string uri = Url.Link("DefaultApi", new { id = ret.PayorId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }


        }
    }
}