using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;

namespace webanhnguyen.Controllers.Admin
{
    public class AdminController : BaseAdminController
    {
        // GET: Item
        private List<tbl_admin> getAdmin(int count)
        {
            return getAdmin(count, "");
        }
        private List<tbl_admin> getAllAdmins()
        {
            return getAdmin(-1, "");
        }
        private List<tbl_admin> getAdmin(int count, String keyword)
        {

            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.tbl_admins.Where(a => a.Username.Contains(keyword)).OrderByDescending(a => a.Username);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.tbl_admins.OrderByDescending(a => a.Username);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }


        }
        private tbl_admin getOneAdmin(string username)
        {
            var item = from ic in data.tbl_admins
                       where ic.Username.Equals(username)
                       select ic;
            if (item == null)
            {
                return new tbl_admin();
            }
            return item.Single();
        }


        // GET: Admin
        public ActionResult Index()
        {
            ViewData["ORDER_COMPLETED_AMOUNT"] = DataHelper.ShoppingCardHelper.getInstance().getPaidOrderAmount(data);
            ViewData["ORDER_AMOUNT"] = DataHelper.ShoppingCardHelper.getInstance().getOrderAmount(data);
            ViewData["MEMBER_AMOUNT"] = DataHelper.AccountHelper.getInstance().getMemberAccountAmount(data);
            ViewData["NEWS_AMOUNT"] = DataHelper.NewsHelper.getInstance().getNewsAmount(data);
            ViewData["ITEM_AMOUNT"] = DataHelper.ProductHelper.getInstance().getProductsAmount(data);
            ViewData["ITEM_CATEGORY_AMOUNT"] = DataHelper.ProductHelper.getInstance().getProductCategoryAmount(data);
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
        
        [HttpGet]
        public ActionResult adminManagementView()
        {
            var listItem = getAdmin(50);
            return View(URLHelper.URL_ADMIN_MANAGEMENT, listItem);
        }
        [HttpPost]
        public ActionResult adminManagementView(FormCollection form, String btnDel)
        {

            if (btnDel != null)
            {
                //Delete all
                DataHelper.AccountHelper.getInstance().deleteAllAdmins(data);
            }

            var keyword = form["keyword"];
            var listItem = getAdmin(10, keyword);
            return View(URLHelper.URL_ADMIN_MANAGEMENT, listItem);
        }

        /*
       * 
       * 
       * 
       */
        [HttpGet]
        public ActionResult adminManagementCreate()
        {
            return View(URLHelper.URL_ADMIN_MANAGEMENT_M, new tbl_admin());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult adminManagementCreate(FormCollection form)
        {
            tbl_admin tic = new tbl_admin();
            var username = form["username"];
            var password = form["password"];
            
            bool err = false;
            if (String.IsNullOrEmpty(username))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập username!\n";
            }
            if (String.IsNullOrEmpty(password))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập mật khẩu!\n";
            }
            if (form["priority"].ToString().Equals("0"))
            {
                err = true;
                ViewData["Error"] += "Vui lòng chọn quyền admin!\n";
            }
            else
            {
                tic.Priority = Int32.Parse(form["priority"]);
            }
            tic.Username = username;
            tic.Password = password;
            if (err == false)
            {
                data.tbl_admins.InsertOnSubmit(tic);
                data.SubmitChanges();
                return RedirectToAction("adminManagementView");
            }
            else
            {
                return View(URLHelper.URL_ADMIN_MANAGEMENT_M, tic);
            }
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult adminManagementEdit(String username)
        {
            return View(URLHelper.URL_ADMIN_MANAGEMENT_M, getOneAdmin(username));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult adminManagementEdit(FormCollection form)
        {
            var username = form["username"];
            if (username == null)
            {
                return adminManagementCreate(form);
            }
            else
            {
                tbl_admin tic = getOneAdmin(username);
                var password = form["password"];
                
                bool err = false;
                if (String.IsNullOrEmpty(username))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập username!\n";
                }
                if (String.IsNullOrEmpty(password))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập mật khẩu!\n";
                }
                if (form["priority"].ToString().Equals("0"))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng chọn quyền admin!\n";
                }
                else
                {
                    tic.Priority = Int32.Parse(form["priority"]);
                }
                tic.Password = password;
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("adminManagementView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_MANAGEMENT_M, tic);
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult adminManagementDelete(String username)
        {
            if (String.IsNullOrEmpty(username))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.tbl_admins.DeleteOnSubmit(getOneAdmin(username));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return adminManagementView();
        }
    }
}