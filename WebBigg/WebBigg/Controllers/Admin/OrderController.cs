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
    public class OrderController : BaseAdminController
    {
        // GET: Order
        public List<OrderDetail> getListOrderDetailById(int id)
        {
            return data.OrderDetails.OrderByDescending(a => a.id).Where(n => n.iddh == id).ToList();
        }

        private List<Order> getOrder(int count)
        {
            return getOrder(count, null);
        }
        private List<Order> getAllOrder()
        {
            return getOrder(-1, null);
        }
        private Order getOneOrder(int id)
        {
            var item = from ic in data.Orders
                       where ic.id == id
                       select ic;
            if (item == null)
            {
                return new Order();
            }
            return item.Single();
        }
        private OrderDetail getOneOrderDetail(int id)
        {
            var item = from ic in data.OrderDetails
                       where ic.id == id
                       select ic;
            if (item == null)
            {
                return new OrderDetail();
            }
            return item.Single();
        }

        public ActionResult OrderSetStatusEnable(int id)
        {
            Order tic = getOneOrder(id);
            tic.status = !tic.status;
            UpdateModel(tic);
            data.SubmitChanges();
            return RedirectToAction("OrderView");
        }
        private List<Order> getOrder(int count, String keyword)
        {
            if (!String.IsNullOrEmpty(keyword))
            {
                var result = data.Orders.Where(a => a.gmail.Contains(keyword)).OrderByDescending(a => a.thoidiemdathang);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
            else
            {
                var result = data.Orders.OrderByDescending(a => a.thoidiemdathang);
                if (count != -1)
                    result.Take(count);
                return result.ToList();
            }
        }

        /*
         * 
         * 
         * 
         */
        public ActionResult Index()
        {
            return OrderView(1);
        }

        [HttpGet]
        public ActionResult OrderDetail(int id)
        {
            var listOrderDetail = getListOrderDetailById(id);
            List<DataHelper.ShoppingCardItemModel> listShoppingCardModels = DataHelper.ShoppingCardHelper.getInstance().getListShoppingCardItemModelFromListOrderDetails(data, listOrderDetail);
            long totalPrice = 0;
            foreach(DataHelper.ShoppingCardItemModel model in listShoppingCardModels)
            {
                totalPrice += (long)model.total;
            }
            Order tic = getOneOrder(id);
            ViewBag.id = tic.id;
            ViewBag.name = tic.tennguoinhan;
            ViewBag.phone = tic.phonenumber;
            ViewBag.address = tic.diachi;
            ViewBag.email = tic.gmail;
            ViewBag.TotalPrice = totalPrice;
            return View(URLHelper.URL_ADMIN_ORDER_DETAIL, listShoppingCardModels);
        }
        
      [HttpPost, ValidateInput(false)]
        public ActionResult OrderDetail(FormCollection form)
        {
            var id = form["id"];
            if (id == null)
            {
                ViewData["Error"] += "Có lỗi xảy ra";
                return RedirectToAction("Index");
            }
            else
            {
                Order tic = getOneOrder(Int32.Parse(id));
                var name = form["TenNguoiNhan"];
                var phone = form["soDT"];
                var address = form["DiaChi"];
                var email = form["yahoo"];
                bool err = false;
                if (String.IsNullOrEmpty(name))
                {
                    err = true;
                    ViewData["Error"] += "Vui lòng nhập tên người nhận hàng!\n";
                }
                tic.tennguoinhan = name;
                tic.phonenumber = phone;
                tic.diachi = address;
                tic.gmail = email;
                if (err == false)
                {
                    UpdateModel(tic);
                    data.SubmitChanges();
                    return RedirectToAction("OrderView");
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.name = tic.tennguoinhan;
                    ViewBag.phone = tic.phonenumber;
                    ViewBag.address = tic.diachi;
                    ViewBag.email = tic.gmail;
                    var listOrderDetail = getListOrderDetailById(Int32.Parse(id));
                    List<DataHelper.ShoppingCardItemModel> listShoppingCardModels = DataHelper.ShoppingCardHelper.getInstance().getListShoppingCardItemModelFromListOrderDetails(data, listOrderDetail);
                    long totalPrice = 0;
                    foreach (DataHelper.ShoppingCardItemModel model in listShoppingCardModels)
                    {
                        totalPrice += (long)model.total;
                    }
                    ViewBag.TotalPrice = totalPrice;
                    return View(URLHelper.URL_ADMIN_ORDER_DETAIL, listShoppingCardModels);

                }
            }
        }

        /*
         * 
         * 
         * 
         */
        [HttpGet]
        public ActionResult OrderView(int ? page)
        {
            int pageNum = (page ?? 1);
            int pageSize = 20;
            var listOrder = getOrder(10);
            return View(URLHelper.URL_ADMIN_ORDER, listOrder.ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        public ActionResult OrderView(FormCollection form, String btnDel, int ? page)
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
                            data.Orders.DeleteOnSubmit(getOneOrder(Int32.Parse(arrayStringCheckedList[i])));
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
            var listOrder = getOrder(10, keyword);
            return View(URLHelper.URL_ADMIN_ORDER, listOrder.ToPagedList(pageNum, pageSize));
        }

        /*
         * 
         * 
         * 
         */
        public ActionResult OrderDelete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.Orders.DeleteOnSubmit(getOneOrder(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return OrderView(1);
        }

        public ActionResult OrderDetailDelete(String id, int idorder)
        {
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.AlertError = "Không xoá được!";
            }
            else
            {
                try
                {
                    data.OrderDetails.DeleteOnSubmit(getOneOrderDetail(Int32.Parse(id)));
                    data.SubmitChanges();
                    ViewBag.AlertSuccess = "Xoá thành công!";
                }
                catch (Exception e)
                {
                    ViewBag.AlertError = "Không xoá được";
                }
            }
            return OrderDetail(idorder);
        }
    }
}