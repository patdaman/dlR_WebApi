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
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_PhysicianEditor")]
#endif
    public class PhysiciansController : ApiController
    {
        PhysiciansProcessor phProc = new PhysiciansProcessor();

        public HttpResponseMessage GetPhysicians()
        {
            try
            {
                return Request.CreateResponse<List<Physician>>(HttpStatusCode.OK, phProc.GetPhysicians());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        [ResponseType(typeof(Physician))]
        public HttpResponseMessage GetPhysician(int id)
        {
            try
            {
                return Request.CreateResponse<Physician>(HttpStatusCode.OK, phProc.GetPhysicians(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        public HttpResponseMessage PutPhysicians(Physician ph)
        {
            try
            {
                Physician p = phProc.UpdatePhysician(ph);
                var response = Request.CreateResponse<Physician>(HttpStatusCode.OK, p);
                string uri = Url.Link("DefaultApi", new { id = p.DoctorId });
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

        /// do not allow insert for Physician editor page
        //public HttpResponseMessage PostPhysicians(Physician ph)
        //{
        //    try
        //    {
        //        Physician p = phProc.CreatePhysician(ph);
        //        var response = Request.CreateResponse<Physician>(HttpStatusCode.Created, p);
        //        string uri = Url.Link("DefaultApi", new { id = p.DoctorId });
        //        response.Headers.Location = new Uri(uri);
        //        return response;
        //    }
        //    catch (SignalException ex)
        //    {
        //        var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
        //        return response;
        //    }

        //}

    }
}