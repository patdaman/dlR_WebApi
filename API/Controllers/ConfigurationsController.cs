using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SignalViewModel;
using CommonUtils;

using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Description;
using CommonUtils.Logging;
using CommonUtils.Build;

namespace SignalAPI.Controllers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A controller for handling the configuration "About" message. This tell you 
    ///             various configuration parameters such as what the current connection strings are, 
    ///             what version of software is running, etc.              
    ///               </summary>
    ///
    /// <remarks>   Dtorres, 20160124. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class ConfigurationsController : ApiController
    {
        string buildTag = BuildUtils.CurrentConfiguration;
        private static readonly log4net.ILog iLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LoggingPatternUser logger = new LoggingPatternUser() { Logger = iLog };

        [ResponseType(typeof(Configuration))]
        public HttpResponseMessage GetConfigurations()
        {   
            try
            {
                Version v = typeof(WebApiApplication).Assembly.GetName().Version;
                Configuration cf = new Configuration()
                {
                    ApiAssemblyVersion = v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Build.ToString() + "." + v.Revision.ToString(),
                    ApiServer = AssemblyUtils.MachineName,
                    ApiBuildTag = buildTag,
                    DB_Finance = getConnectionStringInfo("SGNL_FINANCEEntities"),
                    DB_Warehouse = getConnectionStringInfo("SGNL_WAREHOUSEEntities"),
                    DB_Internal = getConnectionStringInfo("SGNL_INTERNALEntities"),
                    DB_LIS = getConnectionStringInfo("SGNL_LISEntities"),
                    ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")
                };

                var response = Request.CreateResponse<Configuration>(HttpStatusCode.OK, cf);
                return response;
            }
            catch (Exception ex)
            {
                SignalException se = new SignalException(SignalExceptionTypes.NonSignalException_Error, "Cannot Retrieve Configuration", ex);
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, se);
                return response;
            }
            

        }

        private string getConnectionStringInfo(string csName)
        {
            string cstr = System.Configuration.ConfigurationManager.ConnectionStrings[csName].ConnectionString;
            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = cstr;

           string prov = builder["provider connection string"] as string;
            builder.ConnectionString = prov;

            string ret = builder["data source"] as string;
            ret += ":" + builder["initial catalog"] as string;
            return ret;


        }
       
    }
}