using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBigg.Models;

namespace WebBigg.Controllers.Admin
{
    public class SettingController : BaseAdminController
    {
        private GeneralSetting getSetting()
        {
            var setting = from st in data.GeneralSettings
                       select st;
            if (setting == null)
            {
                return new GeneralSetting();
            }
            return setting.Single();
        }
        // GET: Setting
        public ActionResult Index()
        {
            return generalSettingsEdit();
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult generalSettingsEdit()
        {
            return View(URLHelper.URL_ADMIN_SETTING, getSetting());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult generalSettingsEdit(FormCollection form, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2)
        {
            GeneralSetting tic = getSetting();
            var title = form["title"];
            var description = form["description"];
            var keywords = form["keywords"];

            tic.Title = title;
            tic.MetaDescription = description;
            tic.MetaKeywords = keywords;


            if (form["chkClearImg"] != null)
            {
                tic.Favicon = "";
            }
            else if (fileUpload != null)
            {
                var fileName = Path.GetFileName(DateTime.Now.Millisecond + fileUpload.FileName);
                var path = Path.Combine(Server.MapPath(URLHelper.URL_IMAGE_PATH), fileName);
                if (!System.IO.File.Exists(path))
                {
                    fileUpload.SaveAs(path);
                }
                tic.Favicon = fileName;
            }
            
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("Index", "Admin");
        }
    }
}