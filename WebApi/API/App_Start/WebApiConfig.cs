using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using SignalAPILib.Exceptions;

namespace SignalAPI
{
    public static class WebApiConfig
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Registers this object. </summary>
        ///
        /// <remarks>
        /// Ssur, 20150917. Modified from the default to accomodate CORS. Modifications are marked out in
        /// the code.
        /// </remarks>
        ///
        /// <param name="config">   The configuration. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void Register(HttpConfiguration config)
        {

            // Modified from ASP.NET default --------ss/20150917--------------------------------------------
            // Enabling cors for Javascript calls cross domain
            // Note that CORS is globally enabled so not needed to be enabled by each controller
            // Cors can be disabled at the controller/method level using the 
            // [DisableCors] attribute at the relevant level
            // Enable all methods and headers from localhost
            //EnableCorsAttribute cors = new EnableCorsAttribute("http://localhost", "*", "*");
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            cors.SupportsCredentials = true;
            config.EnableCors(cors);


            // End modification from ASP.NET default --------ss/20150917--------------------------------------------

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();


            // Logs all uncaught exceptions to Log4Net. -DT
            config.Services.Add(typeof(IExceptionLogger), new Log4NetExceptionLogger());
        }
    }
}
