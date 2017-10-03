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
    public class NewsController : BaseAdminController
    {
        // GET: News
        private List<tbl_new> getNews(int count)
        {
            return getNews(count, "");
        }
        private List<tbl_new> getAllNews()
        {
            return getNews(-1, "");
        }
        private List<tbl_new> getNews(int count, String keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.tbl_news.Where(a => a.TieuDe.Contains(keyword)).OrderByDescending(a => a.NgayCapNhat);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.tbl_news.OrderByDescending(a => a.NgayCapNhat);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
        }

        public ActionResult NewsSetStatusEnable(int id)
        {
            tbl_new tic = getOneNews(id);
            tic.status = !tic.status;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("newsView");
        }

        private tbl_new getOneNews(int id)
        {
            var news = from ic in data.tbl_news
                       where ic.id == id
                       select ic;
            if (news == null)
            {
                return new tbl_new();
            }
            return news.Single();
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return newsView(1);
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult newsView(int ? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 20;
            var listNews = getNews(10);
            return View(URLHelper.URL_ADMIN_NEWS, listNews.ToPagedList(pageNum,pageSize));
        }
        [HttpPost]
        public ActionResult newsView(FormCollection form, String btnDel, int ? page)
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
                            data.tbl_news.DeleteOnSubmit(getOneNews(Int32.Parse(arrayStringCheckedList[i])));
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
            var listNews = getNews(10, keyword);
            return View(URLHelper.URL_ADMIN_NEWS, listNews.ToPagedList(pageNum,pageSize));
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult newsCreate()
        {
            return View(URLHelper.URL_ADMIN_NEWS_M, new tbl_new());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult newsCreate(FormCollection form, HttpPostedFileBase fileUpload)
        {
            tbl_new tic = new tbl_new();
            var name = form["name"];
            var detail = form["detail"];
            var detail_short = form["detail_short"];


            var title = form["title"];
            var description = form["description"];
            var keyword = form["keyword"];
            tic.title = title;
            tic.description = description;
            tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromNewsName(data, name);


            tic.keyword = keyword;
            bool err = false;
            if (String.IsNullOrEmpty(name))
            {
                err = true;
                ViewData["Error"] += "Vui lòng nhập tên tin tức!\n";
            }
            tic.TieuDe = name;
            tic.status = true;
            tic.NgayCapNhat = DateTime.Now;
            tic.NoiDung = detail;
            tic.TomTat = detail_short;
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
            }else
            {
                tic.UrlHinh = "";
            }
            if (err == false)
            {
                data.tbl_news.InsertOnSubmit(tic);
                data.SubmitChanges();
                return RedirectToAction("newsView");
            }
            else
            {
                return View(URLHelper.URL_ADMIN_NEWS_M, tic);
            }
        }
        /*
         *
         *
         * 
         */
        [HttpGet]
        public ActionResult newsEdit(String id)
        {
            return View(URLHelper.URL_ADMIN_NEWS_M, getOneNews(Int32.Parse(id)));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult newsEdit(FormCollection form, HttpPostedFileBase fileUpload)
        {
            var id = form["id"];
            if (id == null)
            {
                return newsCreate(form, fileUpload);
            }
            else
            {
                tbl_new tic = getOneNews(Int32.Parse(id));
                var name = form["name"];
                var detail = form["detail"];
                var detail_short = form["detail_short"];


                var title = form["title"];
                var description = form["description"];
                var keyword = form["keyword"];
                tic.title = title;
                tic.description = description;
                if (!tic.TieuDe.Equals(name))
                      tic.alias = DataHelper.GeneralHelper.getInstance().getAliasFromNewsName(data, name);


                tic.keyword = keyword;
                bool err = false;
                if (String.IsNullOrEmpty(name))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập tên danh mục!\n";
                }
                tic.TieuDe = name;
                tic.NgayCapNhat = DateTime.Now;
                tic.NoiDung = detail;
                tic.TomTat = detail_short;
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
                else { tic.UrlHinh = ""; }
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("newsView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_NEWS_M, tic);
                }
            }
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult newsDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.tbl_news.DeleteOnSubmit(getOneNews(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return newsView(1);
        }
    }
}