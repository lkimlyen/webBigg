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
    public class ItemController : BaseAdminController
    {
        // GET: ItemCategories
        private List<tbl_product_type> getItemCategories(int count)
        {
            return getItemCategories(count, "");
        }
        private List<tbl_product_type> getAllItemCategories()
        {
            return getItemCategories(-1, "");
        }
        private List<tbl_product_type> getItemCategories(int count, String keyword)
        {
            var result = data.tbl_product_types;
            if (!String.IsNullOrEmpty(keyword))
                result.Where(a => a.TenLoaiSP.Contains(keyword));
            if (count != -1)
                result.Take(count);
            return result.ToList();
        }
        // GET: Item
        private List<tbl_Product> getItem(int count)
        {
            return getItem(count, "");
        }
        private List<tbl_Product> getAllItem()
        {
            return getItem(-1, "");
        }
        private List<tbl_Product> getItem(int count, String keyword)
        {

            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.tbl_Products.Where(a => a.TenSP.Contains(keyword)).OrderByDescending(a => a.NgayCapNhat);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.tbl_Products.OrderByDescending(a => a.NgayCapNhat);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }


        }
        private tbl_Product getOneItem(int id)
        {
            var item = from ic in data.tbl_Products
                       where ic.ID == id
                       select ic;
            if (item == null)
            {
                var product = new tbl_Product();
                product.GiaCu = 0;
                product.GiaHienTai = 0;
                return product;
            }
            return item.Single();
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return itemView(1);
        }

        public ActionResult ItemSetStatusEnable(int id)
        {
            tbl_Product tic = getOneItem(id);
            tic.Status = !tic.Status;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("itemView");
        }
        public ActionResult ItemSetTrangChuEnable(int id)
        {
            tbl_Product tic = getOneItem(id);
            tic.TrangChu = !tic.TrangChu;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("itemView");
        }


        public ActionResult ItemSetCaTuoiMoiNgayEnable(int id)
        {
            tbl_Product tic = getOneItem(id);
            tic.CaTuoiMoiNgay = !tic.CaTuoiMoiNgay;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("itemView");
        }

        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult itemView(int? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 20;
            List<tbl_Product> listItem = getAllItem();

            List<DataHelper.ItemModel> listItemModel = new List<DataHelper.ItemModel>();
            foreach (tbl_Product item in listItem)
            {
                DataHelper.ItemModel model = new DataHelper.ItemModel();
                model.product = item;
                if (item.IDLoaiSP.HasValue)
                    model.categoryName = DataHelper.ProductHelper.getInstance().getProductCategoryById(data, item.IDLoaiSP.Value).TenLoaiSP;
                else
                    model.categoryName = "";
                listItemModel.Add(model);
            }
            return View(URLHelper.URL_ADMIN_ITEM, listItemModel.ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        public ActionResult itemView(FormCollection form, String btnDel, int? page)
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
                            data.tbl_Products.DeleteOnSubmit(getOneItem(Int32.Parse(arrayStringCheckedList[i])));
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
            var listItem = getItem(-1, keyword);
            List<DataHelper.ItemModel> listItemModel = new List<DataHelper.ItemModel>();
            foreach (tbl_Product item in listItem)
            {
                DataHelper.ItemModel model = new DataHelper.ItemModel();
                model.product = item;
                if (item.IDLoaiSP.HasValue)
                    model.categoryName = DataHelper.ProductHelper.getInstance().getProductCategoryById(data, item.IDLoaiSP.Value).TenLoaiSP;
                else
                    model.categoryName = "";
                listItemModel.Add(model);
            }
            return View(URLHelper.URL_ADMIN_ITEM, listItemModel.ToPagedList(pageNum, pageSize));
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult itemCreate()
        {
            var product = new tbl_Product();
            product.GiaCu = 0;
            product.GiaHienTai = 0;
            return View(URLHelper.URL_ADMIN_ITEM_M, new Tuple<tbl_Product, List<tbl_product_type>>(product, getAllItemCategories()));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult itemCreate(FormCollection form, HttpPostedFileBase fileUpload)
        {
            tbl_Product tic = new tbl_Product();
            var name = form["name"];
            var sort = form["sort"];
            var price = form["price"];
            var price2 = form["price2"];
            var online_payment = form["online_payment"];
            var detail = form["detail"];
            var detail_short = form["detail_short"];
            var sold_amount = form["soldamount"];
            var current_amount = form["currentamount"];


            var title = form["title"];
            var description = form["description"];
            var keyword = form["keyword"];
            tic.title = title;
            tic.description = description;
            tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromProductName(data, name);


            tic.keyword = keyword;
            bool err = false;
            if (String.IsNullOrEmpty(name))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập tên sản phẩm!\n";
            }
            if (form["parent"].ToString().Equals("0"))
            {
                err = true;
                ViewData["Error"] += "Vui lòng chọn danh mục!\n";
            }
            else
            {
                tic.IDLoaiSP = Int32.Parse(form["parent"]);
            }
            tic.TenSP = name;
            tic.Status = true;
            tic.TrangChu = true;
            tic.CaTuoiMoiNgay = true;
            tic.NgayCapNhat = DateTime.Now;
            if (!String.IsNullOrEmpty(price))
                tic.GiaCu = Int32.Parse(price);
            else
                tic.GiaCu = 0;
            if (!String.IsNullOrEmpty(price2))
                tic.GiaHienTai = Int32.Parse(price2);
            else
                tic.GiaHienTai = 0;
            if (!String.IsNullOrEmpty(sold_amount))
                tic.SLDaBan = Int32.Parse(sold_amount);
            else
                tic.SLDaBan = 0;
            if (!String.IsNullOrEmpty(current_amount))
                tic.SoLuongTon = Int32.Parse(current_amount);
            else
                tic.SoLuongTon = 0;
            if (tic.GiaHienTai > 0 && tic.GiaCu > tic.GiaHienTai)
                tic.KhuyenMai = (int)(100 - tic.GiaHienTai * new decimal(100) / tic.GiaCu);
            else
                tic.KhuyenMai = 0;
            tic.MoTaCT = detail;
            tic.MoTa = detail_short;
            if (form["chkClearImg"] != null)
            {
                tic.UrlHinh = "";
            }
            else if (fileUpload != null)
            {
                var fileName = Path.GetFileName(DateTime.Now.Millisecond + fileUpload.FileName);
                var path = Path.Combine(Server.MapPath(URLHelper.URL_IMAGE_PATH), fileName);
                if (!System.IO.File.Exists(path))
                {
                    fileUpload.SaveAs(path);
                }
                tic.UrlHinh = fileName;
            }
            if (err == false)
            {
                data.tbl_Products.InsertOnSubmit(tic);
                data.SubmitChanges();
                return RedirectToAction("itemView");
            }
            else
            {
                return View(URLHelper.URL_ADMIN_ITEM_M, new Tuple<tbl_Product, List<tbl_product_type>>(tic, getAllItemCategories()));
            }
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult itemEdit(String id)
        {
            return View(URLHelper.URL_ADMIN_ITEM_M, new Tuple<tbl_Product, List<tbl_product_type>>(getOneItem(Int32.Parse(id)), getAllItemCategories()));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult itemEdit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            var id = form["id"];
            if (id == null)
            {
                return itemCreate(form, fileUpload);
            }
            else
            {
                tbl_Product tic = getOneItem(Int32.Parse(id));
                var name = form["name"];
                var sort = form["sort"];
                var price = form["price"];
                var price2 = form["price2"];
                var online_payment = form["online_payment"];
                var detail = form["detail"];
                var detail_short = form["detail_short"];
                var sold_amount = form["soldamount"];
                var current_amount = form["currentamount"];


                var title = form["title"];
                var description = form["description"];
                var keyword = form["keyword"];
                tic.title = title;
                tic.description = description;
                if (!tic.TenSP.Equals(name))
                    tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromProductName(data, name);

                tic.keyword = keyword;
                bool err = false;
                if (String.IsNullOrEmpty(name))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập tên sản phẩm!\n";
                }
                if (form["parent"].ToString().Equals("0"))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng chọn danh mục!\n";
                }
                else
                {
                    tic.IDLoaiSP = Int32.Parse(form["parent"]);
                }
                tic.TenSP = name;
                tic.NgayCapNhat = DateTime.Now;
                if (!String.IsNullOrEmpty(price))
                    tic.GiaCu = Int32.Parse(price);
                else
                    tic.GiaCu = 0;
                if (!String.IsNullOrEmpty(price2))
                    tic.GiaHienTai = Int32.Parse(price2);
                else
                    tic.GiaHienTai = 0;
                if (!String.IsNullOrEmpty(sold_amount))
                    tic.SLDaBan = Int32.Parse(sold_amount);
                else
                    tic.SLDaBan = 0;
                if (!String.IsNullOrEmpty(current_amount))
                    tic.SoLuongTon = Int32.Parse(current_amount);
                else
                    tic.SoLuongTon = 0;

                if (tic.GiaHienTai > 0 && tic.GiaCu > tic.GiaHienTai)
                    tic.KhuyenMai = (int)(100 - tic.GiaHienTai * new decimal(100) / tic.GiaCu);
                else
                    tic.KhuyenMai = 0;
                tic.MoTaCT = detail;
                tic.MoTa = detail_short;
                if (form["chkClearImg"] != null)
                {
                    tic.UrlHinh = "";
                }
                else if (fileUpload != null)
                {
                    var fileName = Path.GetFileName(DateTime.Now.Millisecond + fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath(URLHelper.URL_IMAGE_PATH), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fileUpload.SaveAs(path);
                    }
                    String imageOlder = URLHelper.URL_IMAGE_PATH + tic.UrlHinh;
                    if (System.IO.File.Exists(imageOlder))
                    {
                        System.IO.File.Delete(imageOlder);
                    }
                    tic.UrlHinh = fileName;
                }
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("itemView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_ITEM_M, new Tuple<tbl_Product, List<tbl_product_type>>(tic, getAllItemCategories()));
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult itemDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.tbl_Products.DeleteOnSubmit(getOneItem(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return itemView(1);
        }
    }
}