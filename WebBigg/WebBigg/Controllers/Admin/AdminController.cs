using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBigg.Models;

namespace WebBigg.Controllers.Admin
{
    public class AdminController : BaseAdminController
    {
        // GET: Item
       
        private tbl_Admin getAdmin()
        {
            var item = from ic in data.tbl_Admins
                       select ic;
            if (item == null)
            {
                return new tbl_Admin();
            }
            return item.Single();
        }


        // GET: Admin
        public ActionResult Index()
        {
            //ViewData["ORDER_COMPLETED_AMOUNT"] = DataHelper.ShoppingCardHelper.getInstance().getPaidOrderAmount(data);
            //ViewData["ORDER_AMOUNT"] = DataHelper.ShoppingCardHelper.getInstance().getOrderAmount(data);
            //ViewData["MEMBER_AMOUNT"] = DataHelper.AccountHelper.getInstance().getMemberAccountAmount(data);
            //ViewData["NEWS_AMOUNT"] = DataHelper.NewsHelper.getInstance().getNewsAmount(data);
            //ViewData["ITEM_AMOUNT"] = DataHelper.ProductHelper.getInstance().getProductsAmount(data);
            //ViewData["ITEM_CATEGORY_AMOUNT"] = DataHelper.ProductHelper.getInstance().getProductCategoryAmount(data);
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var username = form["username"];
            var password = form["password"];
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) &&
                DataHelper.AccountHelper.getInstance().loginAdmin(data, username, password))
            {
                //TODO, save session here
                Session[Constants.KEY_SESSION_ADMIN_USERNAME] = username;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "Vui lòng kiểm tra tên truy cập hoặc mật khẩu.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            DataHelper.AccountHelper.getInstance().logoutAdmin(this);
            return RedirectToAction("Index");
        }


        /*
       * 
       * 
       * 
       */

        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult adminManagementEdit()
        {
            return View(URLHelper.URL_ADMIN_MANAGEMENT_M, getAdmin());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult adminManagementEdit(FormCollection form)
        {
            tbl_Admin tic = getAdmin();
            var password = form["password"];

           
            if (form["password"] != null)
            {
                tic.Password = password;

            }
            UpdateModel(tic);
            data.SubmitChanges();
            DataHelper.AccountHelper.getInstance().logoutAdmin(this);
            return RedirectToAction("Index", "Admin");
        }
    }

}
