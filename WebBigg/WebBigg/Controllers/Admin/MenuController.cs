using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;
using System.IO;

namespace webanhnguyen.Controllers.Admin
{
    public class MenuController : BaseAdminController
    {
        // GET: Menu
        private List<tbl_menu> getMenu(int count)
        {
            return getMenu(count, "");
        }
        private List<tbl_menu> getAllMenu()
        {
            return getMenu(-1, "");
        }
        private List<tbl_menu> getMenu(int count, String keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.tbl_menus.Where(a => a.TenMenu.Contains(keyword));
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.tbl_menus;
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
        }
        private tbl_menu getOneMenu(int id)
        {
            var menu = from ic in data.tbl_menus
                       where ic.id == id
                       select ic;
            if (menu == null)
            {
                return new tbl_menu();
            }
            return menu.Single();
        }

        public ActionResult MenuSetStatusEnable(int id)
        {
            tbl_menu tic = getOneMenu(id);
            tic.Status = !tic.Status;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("menuView");
        }



        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return menuView();
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult menuView()
        {
            var listMenu = getMenu(10);
            return View(URLHelper.URL_ADMIN_MENU, listMenu);
        }
        [HttpPost]
        public ActionResult menuView(FormCollection form, String btnDel)
        {
            if (btnDel != null)
            {
                //Delete all
                DataHelper.ProductHelper.getInstance().deleteAllProductCategory(data);
            }

            var keyword = form["keyword"];
            var listMenu = getMenu(10, keyword);
            return View(URLHelper.URL_ADMIN_MENU, listMenu);
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult menuCreate()
        {
            return View(URLHelper.URL_ADMIN_MENU_M, new tbl_menu());
        }
        [HttpPost]
        public ActionResult menuCreate(FormCollection form, HttpPostedFileBase fileUpload)
        {
            tbl_menu tic = new tbl_menu();
            var TenMenu = form["TenMenu"];
            var url = form["url"];
            var ThuTu = form["ThuTu"];

            bool err = false;
            if (String.IsNullOrEmpty(TenMenu))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập tên menu!\n";
            }
            tic.TenMenu = TenMenu;
            tic.url = url;
            tic.ThuTu = ThuTu != null && !String.IsNullOrEmpty(ThuTu.ToString()) ? Int32.Parse(ThuTu.ToString()) : 1;
            tic.Status = true;

            if (err == false)
            {
                data.tbl_menus.InsertOnSubmit(tic);
                data.SubmitChanges();
                return RedirectToAction("menuView");
            }
            else
            {
                return View(URLHelper.URL_ADMIN_MENU_M, tic);
            }
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult menuEdit(String id)
        {
            return View(URLHelper.URL_ADMIN_MENU_M, getOneMenu(Int32.Parse(id)));
        }
        [HttpPost]
        public ActionResult menuEdit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            var id = form["id"];
            if (id == null)
            {
                return menuCreate(form, fileUpload);
            }
            else
            {
                tbl_menu tic = getOneMenu(Int32.Parse(id));
                var TenMenu = form["TenMenu"];
                var url = form["url"];
                var ThuTu = form["ThuTu"];

                bool err = false;
                if (String.IsNullOrEmpty(TenMenu))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập tên menu!\n";
                }
                tic.TenMenu = TenMenu;
                tic.url = url;
                tic.ThuTu = ThuTu != null && !String.IsNullOrEmpty(ThuTu.ToString()) ? Int32.Parse(ThuTu.ToString()) : 1;
           
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("menuView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_MENU_M, tic);
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult menuDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.tbl_menus.DeleteOnSubmit(getOneMenu(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return menuView();
        }
    }
}