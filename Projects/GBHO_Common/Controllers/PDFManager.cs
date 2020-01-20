using System;
using System.Collections.Generic;
using System.Text;
using ExpertPdf.HtmlToPdf;
using System.IO;
using System.Web;
using System.Drawing;

namespace GBHO_Common.Controllers
{
    /// <summary>
    /// Contains all PDF Generation functions
    /// </summary>
    public static class PDFManager
    {
        /// <summary>
        /// Generates a PDF document from the URL passed in, and returns to user in the HttpResponse (as a download)
        /// </summary>
        /// <param name="inUrl"></param>
        /// <param name="inPasswordProtectRandomly"></param>
        /// <param name="inOrientation"></param>
        /// <param name="inPageSize"></param>
        /// <param name="inServer">The requesting server (Page.Server when requested by an aspx or ascx page)</param>
        /// <param name="inFileName"></param>
        public static void GeneratePDFfromURL(string inUrl, bool inPasswordProtectRandomly, PDFPageOrientation inOrientation, PdfPageSize inPageSize, HttpServerUtility inServer, string inFileName)
        {            
            // get the pdf bytes from html string
            byte[] downloadBytes = GetPDFfromURL(inUrl, inPasswordProtectRandomly, inOrientation, inPageSize, inServer);

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Type", "binary/octet-stream");
            response.AddHeader("Content-Disposition",
                "attachment; filename=" + ((inFileName.Length > 0) ? inFileName : "Report") + ".pdf; size=" + downloadBytes.Length.ToString());
            
            response.BinaryWrite(downloadBytes);                        
        }


        /// <summary>
        /// Get Page As Html string
        /// </summary>
        /// <param name="inUrl"></param>
        /// <param name="inServer"></param>
        /// <returns></returns>
        public static string getPageAsHtml(string inUrl, HttpServerUtility inServer)
        {
            // get the html string for the report
            StringWriter htmlStringWriter = new StringWriter();
            inServer.Execute(inUrl, htmlStringWriter);
            string htmlCodeToConvert = htmlStringWriter.GetStringBuilder().ToString();
            htmlStringWriter.Close();
            return htmlCodeToConvert;
        }


        /// <summary>
        /// Gets a PDF document from the URL passed in, and returns to user in the HttpResponse (as a download)
        /// </summary>
        /// <param name="inUrl"></param>
        /// <param name="inPasswordProtectRandomly"></param>
        /// <param name="inOrientation"></param>
        /// <param name="inPageSize"></param>
        /// <param name="inServer">The requesting server (Page.Server when requested by an aspx or ascx page)</param>
        /// <returns></returns>
        public static byte[] GetPDFfromURL(string inUrl, bool inPasswordProtectRandomly, PDFPageOrientation inOrientation, PdfPageSize inPageSize, HttpServerUtility inServer)
        {
            return GetPDFfromURL(inUrl, inPasswordProtectRandomly, inOrientation, inPageSize, inServer, null, null);
        }


        /// <summary>
        /// Gets a PDF document from the URL passed in, and returns to user in the HttpResponse (as a download)
        /// </summary>
        /// <param name="inUrl"></param>
        /// <param name="inPasswordProtectRandomly"></param>
        /// <param name="inOrientation"></param>
        /// <param name="inPageSize"></param>
        /// <param name="inServer">The requesting server (Page.Server when requested by an aspx or ascx page)</param>
        /// <param name="pdfFooterOptions"></param>
        /// <param name="pdfHeaderOptions"></param>
        /// <returns></returns>
        public static byte[] GetPDFfromURL(string inUrl, bool inPasswordProtectRandomly, PDFPageOrientation inOrientation, PdfPageSize inPageSize, HttpServerUtility inServer, PdfHeaderOptions pdfHeaderOptions, PdfFooterOptions pdfFooterOptions)
        {
            // get the html string for the report           
            string htmlCodeToConvert = getPageAsHtml(inUrl, inServer);


            //initialize the PdfConvert object
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.NavigationTimeout = 120;
            pdfConverter.PdfDocumentOptions.PdfPageSize = inPageSize;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = inOrientation;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            
            pdfConverter.PdfDocumentOptions.LeftMargin = 15;
            pdfConverter.PdfDocumentOptions.TopMargin = 15;
            pdfConverter.PdfDocumentOptions.RightMargin = 0;
            pdfConverter.PdfDocumentOptions.BottomMargin = 15;

            //set header and footer options.
            if (pdfHeaderOptions!=null)
            {                                    
                if (!string.IsNullOrEmpty(pdfHeaderOptions.HeaderText))
                {
                    pdfConverter.PdfDocumentOptions.ShowHeader = true;
                
                    pdfConverter.PdfHeaderOptions.DrawHeaderLine = pdfHeaderOptions.DrawHeaderLine;
                    pdfConverter.PdfHeaderOptions.HeaderText = pdfHeaderOptions.HeaderText;
                    pdfConverter.PdfHeaderOptions.HeaderHeight = pdfHeaderOptions.HeaderHeight;
                    pdfConverter.PdfHeaderOptions.HeaderTextFontSize = pdfHeaderOptions.HeaderTextFontSize;
                    
                    pdfConverter.PdfHeaderOptions.HeaderTextAlign = pdfHeaderOptions.HeaderTextAlign;
                    pdfConverter.PdfHeaderOptions.HeaderTextColor = pdfHeaderOptions.HeaderTextColor;
                }
                pdfConverter.PdfHeaderOptions.ShowOnEvenPages = pdfHeaderOptions.ShowOnEvenPages;
                pdfConverter.PdfHeaderOptions.ShowOnOddPages = pdfHeaderOptions.ShowOnOddPages;
                pdfConverter.PdfHeaderOptions.ShowOnFirstPage = pdfHeaderOptions.ShowOnFirstPage;
            }
            if (pdfFooterOptions!=null)
            {
                pdfConverter.PdfDocumentOptions.ShowFooter = true;
                    
                if (!string.IsNullOrEmpty(pdfFooterOptions.FooterText))
                {
                    pdfConverter.PdfFooterOptions.FooterText = pdfFooterOptions.FooterText;                   
                    pdfConverter.PdfFooterOptions.FooterTextColor = pdfFooterOptions.FooterTextColor;
                    pdfConverter.PdfFooterOptions.FooterTextFontSize = pdfFooterOptions.FooterTextFontSize;                    
                                    
                }
                pdfConverter.PdfFooterOptions.DrawFooterLine = pdfFooterOptions.DrawFooterLine;
                
                if(pdfFooterOptions.FooterHeight > 0)
                    pdfConverter.PdfFooterOptions.FooterHeight = pdfFooterOptions.FooterHeight;
                
                pdfConverter.PdfFooterOptions.ShowPageNumber = pdfFooterOptions.ShowPageNumber;
                pdfConverter.PdfFooterOptions.ShowOnEvenPages = pdfFooterOptions.ShowOnEvenPages;
                pdfConverter.PdfFooterOptions.ShowOnOddPages = pdfFooterOptions.ShowOnOddPages;
                pdfConverter.PdfFooterOptions.ShowOnFirstPage = pdfFooterOptions.ShowOnFirstPage;
            }
            
            pdfConverter.AvoidTextBreak = true;
            pdfConverter.AvoidImageBreak = true;

            pdfConverter.LicenseKey = "HDcuPC4uPC4oLiU8KTIsPC8tMi0uMiUlJSU=";

            if (inPasswordProtectRandomly)
            {
                pdfConverter.AuthenticationOptions.Username = "fujitsu";
                pdfConverter.AuthenticationOptions.Password = new Guid().ToString();
            }

            // set the demo license key
            //pdfConverter.LicenseKey = "put your license key here";

            // get the base url for string conversion which is the url from where the html code was retrieved
            // the base url is a hint for the converter to find the external CSS and images referenced by relative URLs
            string baseUrl = HttpContext.Current.Request.Url.AbsoluteUri;

            // get the pdf bytes from html string
            byte[] downloadBytes = pdfConverter.GetPdfBytesFromHtmlString(htmlCodeToConvert, baseUrl);

            return downloadBytes;
        }
    }
}
