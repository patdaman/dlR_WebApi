using SignalViewModel;
using SignalBusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignalAPI.Controllers
{
#if false
    public class LisCasesController : ApiController
    {
        LisCasesProcessor ProcessLisCase = new LisCasesProcessor();

        [HttpGet]
        public List<Case> GetLisCases ()
        {
            return ProcessLisCase.GetCases();
        }

        [HttpPut]
        public HttpResponseMessage PutLisCases( Case theCase )
        {
            try 
            {
                Case returnCase = ProcessLisCase.UpdateCase(theCase);
                if (returnCase == null)
                    throw new Exception(); 
            }
            catch( Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
#endif
}
