using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Controllers
{

    public class ImagesController : Controller
    {
        public async Task<FileContentResult> RenderStatic(string type, string id, string ext, int w = int.MaxValue)
        {
            bool blSaveAsPng = false;
            byte[] arrByte = System.IO.File.ReadAllBytes(Server.MapPath(string.Format("~/imagestore/{0}", id)));
            if (w > 0)
            {
                arrByte = Wrly.Storage.ImageProcessor.ResizeFile(arrByte, Convert.ToInt32(w), ref blSaveAsPng);
            }
            return new FileContentResult(arrByte, "image/jpg");
        }
    }
}