using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webanhnguyen.Models;
using System.IO;
using PagedList;

namespace webanhnguyen.Controllers.Admin
{
    public class ItemCategoryController : BaseAdminController
    {
        // GET: ItemCategory
        private List<tbl_product_type> getItemCategory(int count)
        {
            return getItemCategory(count, "");
        }
        private List<tbl_product_type> getAllItemCategory()
        {
            return getItemCategory(-1, "");
        }
        private List<tbl_product_type> getItemCategory(int count, String keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.tbl_product_types.Where(a => a.TenLoaiSP.Contains(keyword));
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.tbl_product_types;
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
        }
        private tbl_product_type getOneItemCategory(int id)
        {
            var itemCategory = from ic in data.tbl_product_types
                               where ic.ID == id
                               select ic;
            if (itemCategory == null)
            {
                return new tbl_product_type();
            }
            return itemCategory.Single();
        }

        public ActionResult ItemCategorySetStatusEnable(int id)
        {
            tbl_product_type tic = getOneItemCategory(id);
            tic.Status = !tic.Status;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("itemCategoryView");
        }
        public ActionResult ItemCategorySetTrangChuEnable(int id)
        {
            tbl_product_type tic = getOneItemCategory(id);
            tic.TrangChu = !tic.TrangChu;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("itemCategoryView");
        }

        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return itemCategoryView(1);
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult itemCategoryView(int? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 20;
            var listItemCategory = getItemCategory(10);
            return View(URLHelper.URL_ADMIN_ITEM_CATEGORY, listItemCategory.ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        public ActionResult itemCategoryView(FormCollection form, String btnDel, int? page)
        {
            if (btnDel != null)
            {

                //Delete checked items
                string checkedList = form["chk[]"];
                if (!String.IsNullOrEmpty(checkedList))
                {
                    string[] arrayStringCheckedList = checkedList.Split(new char[] { ',' });
                    for (int i = 0; i < arrayStringCheckedList.Length; i++)
                    {
                        try
                        {
                            data.tbl_product_types.DeleteOnSubmit(getOneItemCategory(Int32.Parse(arrayStringCheckedList[i])));
                            data.SubmitChanges();
                            ViewBag.AlertSuccess = "Xoá thành công!";
                        }
                        catch (Exception e)
                        {
                            ViewBag.AlertError = "Không xoá được";
                        }
                    }
                }
            }

            int pageNum = (page ?? 1);
            int pageSize = 20;
            var keyword = form["keyword"];
            var listItemCategory = getItemCategory(10, keyword);
            return View(URLHelper.URL_ADMIN_ITEM_CATEGORY, listItemCategory.ToPagedList(pageNum, pageSize));
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult itemCategoryCreate()
        {
            return View(URLHelper.URL_ADMIN_ITEM_CATEGORY_M, new tbl_product_type());
        }
        [HttpPost]
        public ActionResult itemCategoryCreate(FormCollection form, HttpPostedFileBase fileUpload)
        {
            tbl_product_type tic = new tbl_product_type();
            var name = form["name"];
            bool err = false;
            if (String.IsNullOrEmpty(name))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập tên danh mục!\n";
            }
            tic.TenLoaiSP = name;
            tic.TrangChu = true;
            tic.Status = true;
            tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromProductTypeName(data, name);
            if (err == false)
            {
                data.tbl_product_types.InsertOnSubmit(tic);
                data.SubmitChanges();
                return RedirectToAction("itemCategoryView");
            }
            else
            {
                return View(URLHelper.URL_ADMIN_ITEM_CATEGORY_M, tic);
            }
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult itemCategoryEdit(String id)
        {
            return View(URLHelper.URL_ADMIN_ITEM_CATEGORY_M, getOneItemCategory(Int32.Parse(id)));
        }
        [HttpPost]
        public ActionResult itemCategoryEdit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            var id = form["id"];
            if (id == null)
            {
                return itemCategoryCreate(form, fileUpload);
            }
            else
            {
                tbl_product_type tic = getOneItemCategory(Int32.Parse(id));
                var name = form["name"];
                bool err = false;
                if (String.IsNullOrEmpty(name))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập tên danh mục!\n";
                }
                if (!tic.TenLoaiSP.Equals(name))
                    tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromProductTypeName(data, name);

                tic.TenLoaiSP = name;
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("itemCategoryView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_ITEM_CATEGORY_M, tic);
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult itemCategoryDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.tbl_product_types.DeleteOnSubmit(getOneItemCategory(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return itemCategoryView(1);
        }
    }
}