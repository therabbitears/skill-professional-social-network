using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastuctures.Utils;
using Wrly.Storage;

namespace Wrly.Controllers
{
    //[Authorize]
    public class UploadController : Controller
    {
        public async Task<ActionResult> AbstractImage()
        {
            var files = Request.Files;
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                if (file.IsImageFile())
                {
                    byte[] Array = new byte[file.ContentLength];
                    file.InputStream.Read(Array, 0, Array.Length);
                    var attribute = ImageProcessor.ValidateImage(Array, string.Empty);
                    return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = attribute };
                }
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new ImageAttribute() { Message = "Unsupported image type, server requires file types[.png, .gif, .jpeg, .jpg, .bmp, .tiff]." } };

            }
            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new ImageAttribute() };
        }


        public async Task<ActionResult> UploadBlob(string existingFileName, string id, int width = int.MaxValue)
        {
            var files = Request.Files;
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                if (file.IsImageFile())
                {
                    byte[] Array = new byte[file.ContentLength];
                    file.InputStream.Read(Array, 0, Array.Length);
                    var attribute = ImageProcessor.ValidateImage(Array, string.Empty);
                    if (attribute.IsValidImage)
                    {
                        var boolPng = false;
                        var bytes = ImageProcessor.ResizeFile(Array, width, ref boolPng);
                        var resultFile = ImageProcessor.UploadImage(bytes, Enums.ImageObject.News, existingFileName, true, Enums.FileType.Image, id, AppConfig.StorageProvider, AppConfig.SiteUrl);
                        attribute.Location = resultFile.FileName;
                        return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = attribute };
                    }
                }
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new ImageAttribute() { Message = "Unsupported image type, server requires file types[.png, .gif, .jpeg, .jpg, .bmp, .tiff]." } };

            }
            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new ImageAttribute() };
        }
    }
}