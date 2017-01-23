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
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_PayorGroupEditor, BillingSuite_PayorEditor")]
#endif
    public class PayorGroupsController : ApiController
    {
        PayorGroupsProcessor procPG = new PayorGroupsProcessor();

        public HttpResponseMessage GetPayorGroups()
        {
            try
            {
                return Request.CreateResponse<List<PayorGroup>>(HttpStatusCode.OK, procPG.GetPayorGroups());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        [ResponseType(typeof(PayorGroup))]
        public HttpResponseMessage GetPayorGroup(int id)
        {
            try
            {
                return Request.CreateResponse<PayorGroup>(HttpStatusCode.OK, procPG.GetPayorGroups(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        public HttpResponseMessage PutPayorGroups(PayorGroup pg)
        {
            try
            {
                PayorGroup tpg = procPG.UpdatePayorGroup(pg);
                var response = Request.CreateResponse<PayorGroup>(HttpStatusCode.OK, tpg);
                string uri = Url.Link("DefaultApi", new { id = tpg.PayorGroupId });
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


        public HttpResponseMessage PostPayorGroups(PayorGroup tpg)
        {
            try
            {
                PayorGroup rpg = procPG.CreatePayorGroup(tpg);
                var response = Request.CreateResponse<PayorGroup>(HttpStatusCode.Created, rpg);
                string uri = Url.Link("DefaultApi", new { id = rpg.PayorGroupId });
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