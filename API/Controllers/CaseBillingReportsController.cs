///-------------------------------------------------------------------------------------------------
// <copyright file="CaseBillingReportsController.cs" company="Signal Genetics Inc.">
// Copyright (c) 2015 Signal Genetics Inc.. All rights reserved.
// </copyright>
// <author>Dtorres</author>
// <date>20151025</date>
// <summary>Implements the case billing reports controller class</summary>
///-------------------------------------------------------------------------------------------------

using SignalBusinessLayer;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using log4net;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_BillingReporter")]
#endif
    public class CaseBillingReportsController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        CaseBillingReportsProcessor reportProcessor = new CaseBillingReportsProcessor();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case billing reports. </summary>
        ///
        /// <remarks>   Dtorres, 20151025. </remarks>
        ///
        /// <param name="fromdate"> The fromdate. </param>
        /// <param name="todate">   The todate. </param>
        ///
        /// <returns>   The case billing reports. </returns>
        ///-------------------------------------------------------------------------------------------------
        public HttpResponseMessage GetCaseBillingReports([FromUri] string fromdate, [FromUri] string todate)
        {
            return InternalCaseBillingReports(fromdate, todate);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets case billing reports. </summary>
        ///
        /// <remarks>   Dtorres, 20151025. </remarks>
        ///
        /// <param name="requestType">  Type of the request. "billingsumary". </param>
        /// <param name="fromdate">     The fromdate. </param>
        /// <param name="todate">       The todate. </param>
        ///
        /// <returns>   The case billing reports. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseBillingReports([FromUri] string requestType, [FromUri] DateTime fromdate, [FromUri] DateTime todate)
        {
            log.InfoFormat("CaseBillingReportsController:GetCaseBillingReports called with requestType={0}, fromdate={1}, todate={2}",
                requestType, fromdate, todate);
            try
            {
                if( requestType.ToLower() == "billingsummary")
                {
                    return GetBillingSummaryReport(fromdate, todate);
                }
                else
                {
                    throw new SignalException(SignalExceptionTypes.RetrieveFailure_Error,
                        String.Format("Unknown request type '{0}' specified.", requestType));
                }                
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }
               


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets reports for a billing event code. </summary>
        ///
        /// <remarks>   Dtorres, 20151025. </remarks>
        ///
        /// <param name="requestType">      Type of the request. Can be "HL7" or "Excel"</param>
        /// <param name="reportType">       Type of the report. </param>
        /// <param name="billingEventCode"> The billing event code. </param>
        ///
        /// <returns>   The case billing reports. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetCaseBillingReports([FromUri] string requestType, [FromUri] string reportType, [FromUri] string billingEventCode)
        {    
            string reporttype = reportType;
            string billingeventname = billingEventCode;
            try
            {
                if (requestType == "Basic")
                return InternalCaseBillingReports(reportType, billingEventCode);

                switch (reporttype)
                    {
                    case "HL7":
                        return GetXifinHL7Report(billingeventname);
                    case "Excel":
                        return GetExcelReport(billingeventname);
                }
                    return null;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets billing summary report. </summary>
        ///
        /// <remarks>   Dtorres, 20151025. </remarks>
        ///
        /// <param name="fromdate"> The fromdate. </param>
        /// <param name="todate">   The todate. </param>
        ///
        /// <returns>   The billing summary report. </returns>
        ///-------------------------------------------------------------------------------------------------

        private HttpResponseMessage GetBillingSummaryReport(DateTime fromdate, DateTime todate)
        {            
            BillingReportCreatorProcessor cp = new BillingReportCreatorProcessor();
            byte[] reportBytes = cp.GetBillingSummaryReport(fromdate, todate);
            return CreateAttachmentResponse(MakeReportName("BillingSummary_", "xlsx"), reportBytes);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Utility for creating a report name with current date in it. </summary>
        ///
        /// <remarks>   Dtorres, 20151025. </remarks>
        ///
        /// <exception cref="SignalException">  Thrown when a Signal error condition occurs. </exception>
        ///
        /// <param name="stem">         The file name stem. </param>
        /// <param name="extension">    The file name extension. </param>
        ///
        /// <returns>   A string. </returns>
        ///-------------------------------------------------------------------------------------------------

        private string MakeReportName(string stem, string extension)
        {
            if (stem == null || stem.Trim() == String.Empty)
                throw new SignalException(SignalExceptionTypes.GeneralFailure_Error, "MakeReportName() input was empty or null.");

            var now = DateTime.Now;
            //e.g. stem01022015.xlsx
            return String.Format("{0}{1}.{2}", stem, now.ToString("MMddyyyy"), extension);
        }

        private HttpResponseMessage InternalCaseBillingReports(string fromdate, string todate)
        {
            DateTime? fdate, tdate;
            ParseDates(fromdate, todate, out fdate, out tdate);

            return Request.CreateResponse<List<CaseBillingReport>>(HttpStatusCode.OK,
                reportProcessor.GetCaseBillingReports(fdate, tdate));

        }

        private static void ParseDates(string fromdate, string todate, out DateTime? fdate, out DateTime? tdate)
        {
            fdate = null;
            tdate = null;
            if (fromdate != null)
                fdate = Convert.ToDateTime(fromdate);

            if (todate != null)
                tdate = Convert.ToDateTime(todate);
        }

        private HttpResponseMessage GetExcelReport(string billingeventname)
        {
            BillingReportCreatorProcessor cp = new BillingReportCreatorProcessor();
            byte[] exf = cp.GetExcelReport(billingeventname);
            return CreateAttachmentResponse(GetBaseFileName(billingeventname) +".xlsx", exf);
        }
                

        private HttpResponseMessage GetXifinHL7Report(string billingeventname)
        {
            BillingReportCreatorProcessor cp = new BillingReportCreatorProcessor();
            String tsvString = cp.GetXifinHL7Report(billingeventname);

            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            tw.WriteLine(tsvString);
            tw.Close();

            return CreateAttachmentResponse(GetBaseFileName(billingeventname)+".tsv", ms.ToArray());            
        }

        private HttpResponseMessage CreateAttachmentResponse(string filename, byte[] fileBytes)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(fileBytes);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = filename;
            return response;
        }


        private string GetBaseFileName(string billingeventname)
        {          
            //remove any invalid characters   
            string regexSearch = new string(Path.GetInvalidFileNameChars());
            var regex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return regex.Replace(billingeventname, "-");
        }
    }
}