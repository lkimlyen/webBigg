using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebBigg.Models;

namespace WebBigg.Controllers
{
    [ActionExcuted]
    public abstract class BaseAdminController : Controller
    {
        public BiGGDataDataContext data = new BiGGDataDataContext();
    }

    public class ActionExcutedAttribute : ActionFilterAttribute
    {
        bool hasAdminLoginSession = false;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            hasAdminLoginSession = DataHelper.AccountHelper.getInstance().checkIsAdminLoggingIn(context.HttpContext);
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            hasAdminLoginSession = DataHelper.AccountHelper.getInstance().checkIsAdminLoggingIn(filterContext.HttpContext);
            bool isCurrentLyInLoginPage = filterContext.ActionDescriptor.ActionName.Equals("Login");
            if (!hasAdminLoginSession && !isCurrentLyInLoginPage)
            {
                //Redirect to page Login Admin
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Admin",
                    action = "Login"
                }));
            }
        }
    }
}