using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;
using System.IO;

namespace webanhnguyen.Controllers.Admin
{
    public class CssController : BaseAdminController
    {
        public ActionResult Index()
        {
            return cssEdit();
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult cssEdit()
        {
            ViewBag.Css = DataHelper.GeneralHelper.getInstance().readCSSFile();
            return View(URLHelper.URL_ADMIN_CSS_EDITING);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult cssEdit(FormCollection form)
        {
            var css = form["css"];
            DataHelper.GeneralHelper.getInstance().saveCSSFile(css);
            return RedirectToAction("Index", "Admin");
        }
    }
}