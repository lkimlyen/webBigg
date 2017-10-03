using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;
using System.IO;

namespace webanhnguyen.Controllers.Admin
{
    public class ShopController : BaseAdminController
    {
        private tbl_shop getShopInformation()
        {
            var shop = from ic in data.tbl_shops
                       select ic;
            if (shop == null)
            {
                return new tbl_shop();
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
            return shopInformationEdit();
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult shopInformationEdit()
        {
            return View(URLHelper.URL_ADMIN_SHOP_INFORMATION, getShopInformation());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult shopInformationEdit(FormCollection form)
        {
            tbl_shop tic = getShopInformation();
            var tenshop = form["tenshop"];
            var emailshop = form["emailshop"];
            var phoneshop1 = form["phoneshop1"];
            var thoigianlamviec1 = form["thoigianlamviec1"];
            var addressshop = form["addressshop"];
            var thoigianlamviec2 = form["thoigianlamviec2"];
            var phoneshop2 = form["phoneshop2"];
            var longtitude = form["longtitude"];
            var lattitude = form["lattitude"];


            var title = form["title"];
            var description = form["description"];
            var keyword = form["keyword"];
            tic.title = title;
            tic.description = description;
            tic.keyword = keyword;

            tic.tenshop = tenshop;
            tic.emailshop = emailshop;
            tic.phoneshop1 = phoneshop1;
            tic.thoigianlamviec1 = thoigianlamviec1;
            tic.thoigianlamviec2 = thoigianlamviec2;
            tic.addressshop = addressshop;
            tic.phoneshop2 = phoneshop2;
            tic.longtitude = longtitude;
            tic.lattitude = lattitude;

            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("Index", "Admin");
        }
    }
}