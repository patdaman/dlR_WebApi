using SignalBusinessLayer;
using SignalViewModel.BaiFile;
using SignalViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace SignalAPI.Controllers
{
#if DEBUG || STAGING || DEVAPP
    [Authorize(Roles = "BillingSuite_Developer")]
#else
    [Authorize(Roles = "BillingSuite_Admin, BillingSuite_PhysicianEditor")]
#endif
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A controller for handling bai Files. </summary>
    ///
    /// <remarks>   Pdelosreyes, 1/4/2016. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class BaiController : ApiController
    {
        private static string BaiFilePath = System.Configuration.ConfigurationManager.AppSettings["BaiFilePath"];
        BaiProcessor baiProc = new BaiProcessor();

        // GET: api/Bai
        // -- Returns a list of BAI files ordered by Descending Post date

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the bai. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <returns>   The bai. </returns>
        ///-------------------------------------------------------------------------------------------------
        public HttpResponseMessage GetBai()
        {
            try
            {
                return Request.CreateResponse<List<BaiFileInfo>>(HttpStatusCode.OK, baiProc.GetBaiList());
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/Bai/5
        // -- Returns the data from a specific BAI file

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets bai file transactions. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The bai file transactions. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetBaiFileTransactions(int id)
        {
            try
            {
                return Request.CreateResponse<List<TransactionInfo>>(HttpStatusCode.OK, baiProc.GetTransactionsByBaiFile(id));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        // GET: api/Bai/5
        // -- Returns the data from a specific BAI file

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a bai. </summary>
        ///
        /// <remarks>   PdlR, 20160123. </remarks>
        ///
        /// <param name="fromDate"> from date. </param>
        /// <param name="toDate">   to date. </param>
        ///
        /// <returns>   The bai. </returns>
        ///-------------------------------------------------------------------------------------------------

        public HttpResponseMessage GetBai(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return Request.CreateResponse<List<TransactionInfo>>(HttpStatusCode.OK, baiProc.GetTransactionList(fromDate, toDate));
            }
            catch (SignalException ex)
            {
                var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                return response;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   (An Action that handles HTTP POST requests) uploads the bai file. </summary>
        ///
        /// <remarks>   PdlR, 1/5/2016. </remarks>
        ///
        /// <exception cref="HttpResponseException">    Thrown when a HTTP Response error condition
        ///                                             occurs.
        /// </exception>
        ///
        /// <returns>   A Task&lt;IHttpActionResult&gt; </returns>
        ///-------------------------------------------------------------------------------------------------

        // POST: api/Bai
        // [HttpPost, Route("api/Bai")]
        public async Task<IHttpActionResult> PostBaiFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var fullFileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                var sections = fullFileName.Split('\\');
                var fileName = sections[sections.Length - 1];
                var fileExt = fileName.Split('.')[1];
                var buffer = await file.ReadAsStreamAsync();
                try
                {
                    if (File.Exists(BaiFilePath + fileName))
                        throw new SignalException(SignalExceptionTypes.CreateFailure_Exists, "File Name already exists: " + fileName);
                    using (var fileStream = File.Create(BaiFilePath + fileName))
                    {
                        buffer.CopyTo(fileStream);
                    }
                    var newBaiObject = new BaiFile();
                    if (fileExt == "bai")
                    {
                        newBaiObject = AppDataLib.BaiParser.BAIProcessor.ParseBaiFile(BaiFilePath + fileName);
                    }
                    else if (fileExt == "csx")
                    {
                        newBaiObject = baiProc.ParseCITICsxToBaiFormat(BaiFilePath + fileName);
                    }
                    else
                        throw new SignalException(SignalExceptionTypes.CreateFailure_Exists, "File type not valid: " + fileExt);

                    var savedBaiObject = baiProc.CreateBaiFile(newBaiObject);
                }
                catch (SignalException ex)
                {
                    var response = Request.CreateResponse<SignalException>(HttpStatusCode.BadRequest, ex);
                    return ResponseMessage(response);
                }
            }
            return Ok();
        }

        // PUT: api/Bai/5
        // -- Update Disabled
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/Bai/5
        // -- Delete Disabled
        //public void Delete(int id)
        //{
        //}
    }
}
