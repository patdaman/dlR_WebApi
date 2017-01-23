///-------------------------------------------------------------------------------------------------
// <copyright file="BillingEventsController.cs" company="Signal Genetics Inc.">
// Copyright (c) 2015 Signal Genetics Inc.. All rights reserved.
// </copyright>
// <author>Ssur</author>
// <date>20151007</date>
// <summary>Implements the billing events controller class</summary>
///-------------------------------------------------------------------------------------------------

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
using SignalBusinessLayer;


namespace SignalAPI.Controllers
{

#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_BillingReporter")]
#endif

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A controller for handling billing events. </summary>
    ///
    /// <remarks>   Ssur, 20151007. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class BillingEventsController : ApiController
    {
        private const string DefaultApi = "DefaultApi";

        BillingEventsProcessor processor = new BillingEventsProcessor();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets billing events. </summary>
        ///
        /// <remarks>   Ssur, 20151007. </remarks>
        ///
        /// <returns>   The billing events. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetBillingEvents()
        {
            try
            {
                List<BillingEvent> list = processor.GetBillingEvents();
                var response = Request.CreateResponse<List<BillingEvent>>(HttpStatusCode.Created, list);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Validate a billing aggregate in a specified date range. </summary>
        ///
        /// <remarks>   Ssur, 20151007. </remarks>
        ///
        /// <param name="fromDate">         from date. </param>
        /// <param name="toDate">           to date. </param>
        /// <param name="billingAggregate"> The billing aggregate. </param>
        ///  <param name="datetype">           to date. </param>
        ///
        /// <returns>   The billing event. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetBillingEvent(DateTime? fromDate,
            DateTime? toDate,
            string billingAggregate,
            string datetype
            )
        {
            try
            {
                BillingEvent returnedItem = processor.ValidateBillingEvent(fromDate, toDate, billingAggregate, datetype);
                var response = Request.CreateResponse<BillingEvent>(HttpStatusCode.Accepted, returnedItem);
                string uri = Url.Link(DefaultApi, new { id = returnedItem.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Puts case list for billing. </summary>
        ///
        /// <remarks>   Ssur, 20151015. </remarks>
        ///
        /// <param name="caseList">         List of cases. </param>
        /// <param name="billingAggregate"> The billing aggregate. </param>
        /// <param name="comment">          The comment. </param>
        ///  <param name="xifinCreateAccession">     if true, cases will be pushed to Xifin through the CreateAccession API. </param>
        ///
        /// <returns>   A HttpResponseMessage. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage PutCaseListForBilling(List<string> caseList,
           string billingAggregate,
           string comment,
           bool xifinCreateAccession)
        {            
            try
            {

                BillingEvent returnedItem;
                if (caseList == null || caseList.Count == 0)
                {
                    returnedItem = GetInvalidBillignEvent(billingAggregate);
                }
                else
                {
                    string userName = HttpContext.Current.User.Identity.Name;
                     returnedItem = processor.CreateBillingEvent(caseList, userName, billingAggregate, comment, xifinCreateAccession);
                }

                var response = Request.CreateResponse<BillingEvent>(HttpStatusCode.Created, returnedItem);
                string uri = Url.Link(DefaultApi, new { id = returnedItem.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        private static BillingEvent GetInvalidBillignEvent(string billingAggregate)
        {
            // \todo when we catch exceptiosn on the front end we should throw exception, for now just return an empty case. -dt
            //throw new SignalException(SignalExceptionTypes.CreateFailure_Error, "0 cases have been billed.");
            return new BillingEvent
            {
                Id = -1,
                BillingEventCode = "NOT CREATED",
                BillingDate = DateTime.Now,
                BilledCaseCount = 0,
                AttemptedBilledCaseCount = 0,
                MessageType = "INVALID",
                BillingAggregate = billingAggregate
            };
        }
    }
}