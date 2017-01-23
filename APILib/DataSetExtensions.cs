///-------------------------------------------------------------------------------------------------
// <copyright file="DataSetExtensions.cs" company="Signal Genetics Inc.">
// Copyright (c) 2016 Signal Genetics Inc.. All rights reserved.
// </copyright>
// <author>Dtorres</author>
// <date>20160331</date>
// <summary>Implements the data set extensions class</summary>
///-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using PathCentralApiLib;
using PathCentralApiLib.Pdf;

namespace SignalAPILib
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A data set extensions. </summary>
    ///
    /// <remarks>   Dtorres, 20160331. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public static class DataSetExtensions
    {
        

        public static FileHelper GetPathCentralPDFFile( this List<PathCentralPdf> doclist )
        {
            if (doclist == null)
                throw new ArgumentNullException(); 

            FileHelper fileHelper = new FileHelper();
            if (doclist.Count >= 1)
            {
                fileHelper.ContentType = "PDF";
                fileHelper.FileName = doclist[0].DocumentName;
                fileHelper.FileBytes = doclist.GetPdfsAsOneDocument();
                fileHelper.Status = FileHelper.FileHelperStatus.OK;
            }
            else
            {
                fileHelper.Status = FileHelper.FileHelperStatus.EmptyDownload;
            }
            return fileHelper; 

        }
        


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   A DataSet extension method that gets path central PDF file. </summary>
        ///
        /// <remarks>   Dtorres, 20160331. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="ds">           The ds to act on. </param>
        /// <param name="colname">      The colname. </param>
        /// <param name="tablename">    The tablename. </param>
        ///
        /// <returns>   The path central PDF file. </returns>
        ///-------------------------------------------------------------------------------------------------
        [Obsolete("Not used.",false)]
        public static FileHelper GetPathCentralPDFFile(this DataSet ds, string colname = "Column1", string tablename = "Table")
        {
            // See the following example: http://developers.itextpdf.com/examples/itext-action-second-edition/chapter-6
            // Search for the example called Contenate.java. -DT 

            DataTable table = ds.Tables[tablename];
            FileHelper fileHelper = new FileHelper();
            if (table.Rows.Count > 0) //if payload has data
            {
                try
                {
                    fileHelper.ContentType = @"application/" + table.Rows[0]["DocumentTypeName"];
                    fileHelper.FileName = table.Rows[0]["DocumentName"] as string;

                    //create final PDF shell                     
                    MemoryStream finalOutputStream = new MemoryStream();
                    var finalDocument = new iTextSharp.text.Document();
                    var pdfCopier = new iTextSharp.text.pdf.PdfCopy(finalDocument, finalOutputStream);
                    finalDocument.Open();
                    
                    //Loop over source PDFs                     
                    foreach (DataRow r in table.Rows)
                    {
                        //open source PDF 
                        byte[] sourceDocumentBytes = r["DocumentBinary"] as byte[];                        
                        var pdfReader = new iTextSharp.text.pdf.PdfReader(sourceDocumentBytes);

                        //loop over pages 
                        for (int iPage = 1; iPage <= pdfReader.NumberOfPages; iPage++)
                        {
                            pdfCopier.AddPage(pdfCopier.GetImportedPage(pdfReader, iPage));
                        }
                        pdfCopier.FreeReader(pdfReader);
                        pdfReader.Close();
                    }
                    finalDocument.Close();
                    fileHelper.FileBytes = finalOutputStream.GetBuffer();                    
                    fileHelper.Status = FileHelper.FileHelperStatus.OK;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                fileHelper.Status = FileHelper.FileHelperStatus.EmptyDownload;
            }
            return fileHelper;
#if false
            //USES PDF SHARP
            DataTable dt = ds.Tables[tablename];
            FileHelper fh = new FileHelper();
            if (dt.Rows.Count > 0)
            {
                try
                {
                    fh.ContentType = @"application/" + dt.Rows[0]["DocumentTypeName"];
                    fh.FileName = dt.Rows[0]["DocumentName"] as string;

                    // aproach for combining pdfs
                    PdfDocument finalPdf = new PdfDocument();
                    foreach (DataRow r in dt.Rows)
                    {
                        byte[] documentBytes = r["DocumentBinary"] as byte[];
                        MemoryStream stream = new MemoryStream(documentBytes);
                        PdfDocument document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                        for (int i = 0; i < document.PageCount; i++)
                            finalPdf.AddPage(document.Pages[i]);
                    }
                    MemoryStream outpuStream = new MemoryStream();
                    finalPdf.Save(outpuStream);
                    fh.FileBytes = outpuStream.GetBuffer();                    
                    fh.Status = FileHelper.FileHelperStatus.OK;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                fh.Status = FileHelper.FileHelperStatus.EmptyDownload;
            }
            return fh;
#endif 
        }
    }
}
