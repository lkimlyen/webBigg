using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;
using System.IO;

namespace webanhnguyen.Controllers.Admin
{
    public class HeaderController : BaseAdminController
    {
        private tbl_header getHeader()
        {
            var shop = from ic in data.tbl_headers
                       select ic;
            if (shop == null)
            {
                return new tbl_header();
            }
            return shop.Single();
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return headerEdit();
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult headerEdit()
        {
            return View(URLHelper.URL_ADMIN_HEADER, getHeader());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult headerEdit(FormCollection form, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2)
        {
            tbl_header tic = getHeader();
            var tittle = form["tittle"];
            var phone1 = form["phone1"];
            var phone2 = form["phone2"];

            tic.tittle = tittle;
            tic.phone1 = phone1;
            tic.phone2 = phone2;


            if (form["chkClearImg"] != null)
            {
                tic.image = "";
            }
            else if (fileUpload != null)
            {
                var fileName = Path.GetFileName(DateTime.Now.Millisecond + fileUpload.FileName);
                var path = Path.Combine(Server.MapPath(URLHelper.URL_IMAGE_PATH), fileName);
                if (!System.IO.File.Exists(path))
                {
                    fileUpload.SaveAs(path);
                }
                tic.image = fileName;
            }

            if (form["chkClearImgIcon"] != null)
            {
                tic.shortcuticon = "";
            }
            else if (fileUpload2 != null)
            {
                var fileName = Path.GetFileName(DateTime.Now.Millisecond + fileUpload2.FileName);
                var path = Path.Combine(Server.MapPath(URLHelper.URL_IMAGE_PATH), fileName);
                if (!System.IO.File.Exists(path))
                {
                    fileUpload2.SaveAs(path);
                }
                tic.shortcuticon = fileName;
            }


            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("Index", "Admin");
        }
    }
}