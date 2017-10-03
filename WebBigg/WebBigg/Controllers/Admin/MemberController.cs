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
    public class MemberController : BaseAdminController
    {
        // GET: Member
        private List<Customer> getMember(int count)
        {
            return getMember(count, "");
        }
        private List<Customer> getAllMember()
        {
            return getMember(-1, "");
        }
        private List<Customer> getMember(int count, String keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.Customers.Where(a => a.email.Contains(keyword)).OrderByDescending(a => a.date_added);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.Customers.OrderByDescending(a => a.date_added);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
        }
        private Customer getOneMember(int id)
        {
            var Member = from ic in data.Customers
                         where ic.id == id
                         select ic;
            if (Member == null)
            {
                return new Customer();
            }
            return Member.Single();
        }
        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return MemberView(1);
        }
        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult MemberView(int ? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 20;
            var listMember = getMember(10);
            return View(URLHelper.URL_ADMIN_MEMBER, listMember.ToPagedList(pageNum,pageSize));
        }
        [HttpPost]
        public ActionResult MemberView(FormCollection form, String btnDel, int ? page)
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
                            data.Customers.DeleteOnSubmit(getOneMember(Int32.Parse(arrayStringCheckedList[i])));
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
            var listMember = getMember(10, keyword);
            return View(URLHelper.URL_ADMIN_MEMBER, listMember.ToPagedList(pageNum, pageSize));
        }


        [HttpGet]
        public ActionResult MemberDetailView(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Có lỗi xảy ra!";
            }
            else
            {
                return View(URLHelper.URL_ADMIN_MEMBER_DETAIL, getOneMember(Int32.Parse(id)));
            }
            return MemberView(1);
        }
        /*
      *
      *
      * 
      */
        [HttpPost, ValidateInput(false)]
        public ActionResult MemberDetailView(FormCollection form, HttpPostedFileBase fileUpload)
        {
            var id = form["id"];
            if (id == null)
            {
                return MemberView(form, null,1);
            }
            else
            {
                Customer tic = getOneMember(Int32.Parse(id));
                var name = form["name"];
                var email = form["email"];
                var address = form["address"];
                var phone = form["phone"];

                bool err = false;
                if (String.IsNullOrEmpty(email))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập email khách hàng!\n";
                }
                else
                    tic.email = email;
                tic.name = name;
                tic.address = address;
                tic.moblie = phone;
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("MemberView");
                }
                else
                {
                    return View(URLHelper.URL_ADMIN_MEMBER_DETAIL, getOneMember(Int32.Parse(id)));
                }
            }
        }

        public ActionResult MemberDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.Customers.DeleteOnSubmit(getOneMember(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return MemberView(1);
        }
    }
}