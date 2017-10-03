using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBigg.Controllers
{
    public class DataHelper
    {
        
        //Helper classes
        public class GeneralHelper
        {
            static GeneralHelper instance;
            public static GeneralHelper getInstance()
            {
                if (instance == null)
                {
                    instance = new GeneralHelper();
                }
                return instance;
            }
            public string RemoveUnicode(string text)
            {
                string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ",};
                string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y",};
                for (int i = 0; i < arr1.Length; i++)
                {
                    text = text.Replace(arr1[i], arr2[i]);
                    text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
                }

                string chuoikytudacbiet = "~!@#$%^&*()Ơ}|:\"?><\\/-_+=[]';`.,";
                char[] mangkytudacbiet = chuoikytudacbiet.ToCharArray();
                for(int i = 0; i < mangkytudacbiet.Length; i++)
                {
                    text = text.Replace(mangkytudacbiet[i],'-');
                }
                text = text.Replace(" ", "-");
                return text;
            }
            
        }

       
        //public class ProductHelper
        //{
        //    static ProductHelper instance;
        //    public static ProductHelper getInstance()
        //    {
        //        if (instance == null)
        //        {
        //            instance = new ProductHelper();
        //        }
        //        return instance;
        //    }

        //    public void deleteAllProductCategory(Models.databaseDataContext data)
        //    {
        //        deleteAllProduct(data);

        //        data.tbl_product_types.DeleteAllOnSubmit(data.tbl_product_types);
        //        data.SubmitChanges();
        //    }

        //    public void deleteAllProduct(Models.databaseDataContext data)
        //    {
        //        //ShoppingCardHelper.getInstance().deleteAllOrderDetails(data);

        //        data.tbl_Products.DeleteAllOnSubmit(data.tbl_Products);
        //        data.SubmitChanges();
        //    }

        //    public int getProductsAmount(Models.databaseDataContext data)
        //    {
        //        return data.tbl_Products.Count();
        //    }

        //    public int getProductCategoryAmount(Models.databaseDataContext data)
        //    {
        //        return data.tbl_product_types.Count();
        //    }


        //    public Models.tbl_Product getProductById(Models.databaseDataContext data, int id)
        //    {
        //        Models.tbl_Product result = data.tbl_Products.Where(n => n.ID == id).Single();
        //        return result;
        //    }

        //    public Models.tbl_product_type getProductCategoryById(Models.databaseDataContext data, int id)
        //    {
        //        Models.tbl_product_type result = data.tbl_product_types.Where(n => n.ID == id).Single();
        //        return result;
        //    }

        //    public List<Models.tbl_Product> getListAllProducts(Models.databaseDataContext data)
        //    {
        //        return data.tbl_Products.OrderByDescending(a => a.NgayCapNhat).ToList();
        //    }

        //    public List<Models.tbl_Product> getListProductsByCategory(Models.databaseDataContext data, int idProductType)
        //    {
        //        return data.tbl_Products.OrderByDescending(a => a.NgayCapNhat).Where(n => n.IDLoaiSP == idProductType).ToList();
        //    }

        //    public List<Models.tbl_Product> getListOtherProductsByCategory(Models.databaseDataContext data, int id, int idProductType)
        //    {
        //        return data.tbl_Products.OrderByDescending(a => a.NgayCapNhat).Where(n => n.IDLoaiSP == idProductType && n.ID != id).ToList();
        //    }
        //}

        public class AccountHelper
        {
            static AccountHelper instance;
            public static AccountHelper getInstance()
            {
                if (instance == null)
                {
                    instance = new AccountHelper();
                }
                return instance;
            }
            
            public bool loginAdmin(Models.BiGGDataDataContext data, string username, string password)
            {
                return checkThisAdminAccountExist(data, username, password);
            }
            

            public bool checkThisAdminAccountExist(Models.BiGGDataDataContext data, string username, string password)
            {
                var result = data.tbl_Admins.Where(a => a.Username.Equals(username) && a.Password == password);
                if (result.Count() > 0)
                {
                    return true;
                }
                return false;
            }

            public bool checkIsAdminLoggingIn(HttpContextBase context)
            {
                Object session = context.Session[Constants.KEY_SESSION_ADMIN_USERNAME];
                if (session != null && !String.IsNullOrEmpty(session.ToString()))
                {
                    return true;
                }
                return false;
            }

            public void logoutAdmin(BaseAdminController context)
            {
                context.Session[Constants.KEY_SESSION_ADMIN_USERNAME] = null;
            }

          
            public void deleteAllAdmins(Models.BiGGDataDataContext data)
            {
                data.tbl_Admins.DeleteAllOnSubmit(data.tbl_Admins);
                data.SubmitChanges();
            }
        }

    }
}