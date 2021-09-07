#region Copyright Syncfusion Inc. 2001-2015.
// Copyright Syncfusion Inc. 2001-2015. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Syncfusion.EJ2.PdfViewer;
using System.Web.Mvc;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.Drawing;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Redaction;
using System.Web.Http.Cors;

namespace WebApiSample.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PdfViewerController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        public object Load(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    string documentPath = GetDocumentPath(jsonObject["document"]);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);

                        stream = new MemoryStream(bytes);
                    }
                    else
                    {
                        return (jsonObject["document"] + " is not found");
                    }
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }
            jsonResult = pdfviewer.Load(stream, jsonObject);
            return (JsonConvert.SerializeObject(jsonResult));
        }

        [System.Web.Mvc.HttpPost]
        public object Bookmarks(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            var jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return (jsonResult);
        }

        [System.Web.Mvc.HttpPost]
        public object RenderPdfPages(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetPage(jsonObject);
            return (JsonConvert.SerializeObject(jsonResult));
        }

        [System.Web.Mvc.HttpPost]
        public object RenderThumbnailImages(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return (JsonConvert.SerializeObject(result));
        }

        [System.Web.Mvc.HttpPost]
        public object RenderPdfTexts(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object result = pdfviewer.GetDocumentText(jsonObject);
            return (JsonConvert.SerializeObject(result));
        }

        [System.Web.Mvc.HttpPost]
        public object RenderAnnotationComments(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return (jsonResult);
        }

        [System.Web.Mvc.HttpPost]
        public object Unload(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            pdfviewer.ClearCache(jsonObject);
            return ("Document cache is cleared");
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage Download(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            //  string base64String = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None)[1];// error appearse at this line
            string[] temp = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None);
            MemoryStream stream = new MemoryStream();
            byte[] bytes = new byte[0];
            if (!string.IsNullOrEmpty(documentBase))
            {
                // if (temp.Length > 0)

                bytes = Convert.FromBase64String(temp[1]);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(bytes);

                foreach (PdfLoadedPage pageBase in loadedDocument.Pages)
                {

                    foreach (PdfAnnotation annots in pageBase.Annotations)
                    {
                        var annotationFlags = annots.AnnotationFlags;
                        var bounds = annots.Bounds;
                        var annotationName = annots.Name;
                        var subject = annots.Subject;
                        var text = annots.Text;
                        var type = annots.GetType().Name;
                        var color = annots.Color;
                        PdfColor pdfColor = new PdfColor(Color.Gray); /*Color grey*/


                        if (subject != null)
                        {

                            if ((subject.Equals(PdfLoadedAnnotationTypes.Highlight.ToString()) || subject.Equals("Rectangle")))
                            {
                                var colorCompare = color == pdfColor;
                                PdfRedaction redaction = new PdfRedaction(bounds, Color.Black);
                                //redaction.Appearance.Graphics.DrawRectangle(Graphics.)
                                pageBase.Redactions.Add(redaction);
                            }
                        }

                    }
                }


                MemoryStream stream1 = new MemoryStream();
                loadedDocument.Save(stream);
                stream.Position = 0;

                bytes = stream.ToArray();

                documentBase = string.Concat("data:application/pdf;base64,", Convert.ToBase64String(bytes));
            }




            string filePath = HttpContext.Current.Request.PhysicalApplicationPath + "App_Data\\Data\\PdfWithAnnots";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileName = jsonObject["documentId"];
            string fullPath = Path.Combine(filePath, fileName);

            File.WriteAllBytes(fullPath, bytes);


            return (GetPlainText(String.Empty));



            //  if (base64String != null || base64String != string.Empty)
            //  {
            //      byte[] byteArray = Convert.FromBase64String(base64String);
            //     /// System.IO.File.WriteAllBytes("f:\\output.pdf", byteArray);
            //  }
            ////  return (GetPlainText(String.Empty));
            // return (GetPlainText(documentBase));
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage SaveinDB(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            string base64String = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None)[1];// error appearse at this line
            if (base64String != null || base64String != string.Empty)
            {
                byte[] byteArray = Convert.FromBase64String(base64String);
                //we can get the document and store in DB as per the requirement.

            }

            return (GetPlainText(String.Empty));
            // return (GetPlainText(documentBase));
        }

        [System.Web.Mvc.HttpPost]
        public object PrintImages(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return (pageImage);
        }
        [System.Web.Mvc.HttpPost]
        //Post action to export annotations
        public HttpResponseMessage ExportAnnotations(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string jsonResult = pdfviewer.GetAnnotations(jsonObject);
            //  return ((jsonResult));
            return (GetPlainText(jsonResult));
        }

        [System.Web.Mvc.HttpPost]
        //Post action to import annotations
        public object ImportAnnotations(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string jsonResult = string.Empty;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(jsonObject["fileName"]);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    jsonResult = System.IO.File.ReadAllText(documentPath);
                }
                else
                {
                    return (JsonConvert.SerializeObject(jsonObject["document"] + " is not found"));
                }
            }
            return (JsonConvert.SerializeObject(jsonResult));
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage ExportFormFields(Dictionary<string, string> jsonObject)

        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string jsonResult = pdfviewer.ExportFormFields(jsonObject);
            //   return (JsonConvert.SerializeObject(jsonResult));
            return (GetPlainText(jsonResult));
        }

        [System.Web.Mvc.HttpPost]
        public object ImportFormFields(Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.ImportFormFields(jsonObject);
            return (JsonConvert.SerializeObject(jsonResult));
        }

        private HttpResponseMessage GetPlainText(string pageImage)
        {
            var responseText = new HttpResponseMessage(HttpStatusCode.OK);
            responseText.Content = new StringContent(pageImage, System.Text.Encoding.UTF8, "text/plain");
            return responseText;
        }
        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                var path = HttpContext.Current.Request.PhysicalApplicationPath;
                if (System.IO.File.Exists(path + "Data\\" + document))
                    documentPath = path + "Data\\" + document;
            }
            else
            {
                documentPath = document;
            }
            return documentPath;
        }

        // GET api/values
        [System.Web.Mvc.HttpPost]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }

}