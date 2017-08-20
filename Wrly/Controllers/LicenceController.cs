using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Wrly.Models.External;

namespace Wrly.Controllers
{
    public class LicenceController : Controller
    {
        static string key = "H0vE'rH#tKe????d0N;T,rbEAk,lPaese";
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Generate(LicenceFile model)
        {
            if (ModelState.IsValid)
            {
                var hash = Guid.NewGuid().ToString();
                string data = string.Format("{0}|{1}|{2}|{3}{4}", model.CompanyName, model.TotalUsers, model.Stamp, model.LicenceCategory, model.Validity);
                var stringCiper = StringCiper.Encrypt(data, key);
                model.LicenceString = stringCiper;
                model.SaltHash = hash;

                try
                {
                    SerializeObject<LicenceFile>(model, Server.MapPath("~/Licences/" + hash + ".xml"));
                }
                catch
                {
                    return null;
                }

                return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return null;
        }


        public JsonResult Validate(LicenceFile model)
        {
            if (ModelState.IsValid)
            {
                model.LicenceString = model.LicenceString.Replace(" ", "+");
                string message = string.Empty;
                var isValid = IsValid(model, out message);
                model.Result = isValid==true;
                return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return null;
        }

        [HttpPost]
        public JsonResult Activate(LicenceFile model)
        {
            if (ModelState.IsValid)
            {
                string message = string.Empty;
                var isValid = IsValid(model, out message);
                if (isValid == true)
                {
                    model.DateOfActivation = DateTime.Now;
                    model.DateOfExpiry = DateTime.Now.AddMonths(Convert.ToInt32(model.Validity));
                    model.Message = string.Format("Licence has been activated with the effective from {0} and valid till {1}", model.DateOfActivation, model.DateOfExpiry);
                    model.Result = true;
                    SerializeObject<LicenceFile>(model, Server.MapPath("~/Licences/" + model.SaltHash + ".xml"));
                }
                return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return null;
        }

        //[HttpPost]
        //public JsonResult LockCode(string pcName)
        //{
        //    if (string.IsNullOrEmpty(pcName))
        //    {
        //        pcName = "fuckOffAndDontShowMeYourFaceAgainDontTeachYourFatherHowToFuckYouBC_";
        //    }
        //    else
        //    {
        //        pcName = "MyLovelyPCHere$$$$^^^^^^_";
        //    }
        //    return new JsonResult() { Data = new { locked = true, key = pcName + "H0v$rJ%tkEF><?" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        private bool? IsValid(LicenceFile model, out string message)
        {
            try
            {
                if (System.IO.File.Exists(Server.MapPath("~/Licences/" + model.SaltHash + ".xml")))
                {
                    var objectDes = Deserialize(Server.MapPath("~/Licences/" + model.SaltHash + ".xml"));
                    if (objectDes != null)
                    {

                        if (objectDes.CompanyName.Equals(model.CompanyName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (objectDes.LicenceCategory.Equals(model.LicenceCategory, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (objectDes.LicenceString.Equals(model.LicenceString, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (objectDes.SaltHash.Equals(model.SaltHash, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (objectDes.Stamp.Equals(model.Stamp, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (objectDes.TotalUsers.Equals(model.TotalUsers, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                if (objectDes.Validity.Equals(model.Validity, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    if (Convert.ToDateTime(model.Stamp).AddMonths(Convert.ToInt32(model.Validity)) <= DateTime.Now)
                                                    {
                                                        message = "The licence is not valid";
                                                        return null;
                                                    }
                                                    message = "Licence validation successfull";
                                                    return true;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    message = "Invalid licence, either it has been modified or obtained from unauthorized source.";
                    return false;
                }
            }
            catch
            {
                message = "Invalid licence, either it has been modified or obtained from unauthorized source.";
                return null;
            }
            message = "Invalid licence, either it has been modified or obtained from unauthorized source.";
            return null;
        }

        private LicenceFile Deserialize(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LicenceFile));
                using (StreamReader reader = new StreamReader(path))
                {
                    var file = ((LicenceFile)serializer.Deserialize(reader));
                    reader.Close();
                    return file;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }
            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }
    }
}