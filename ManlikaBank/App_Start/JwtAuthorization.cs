using Manlika_WebApi.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManlikaBank.App_Start
{
    public class JwtAuthorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"].Value;

            if (token != null)
            {
                var userName = JwtAuthenticationManager.ValidateToken(token);
                if (userName == null)
                {
                    filterContext.Result = new HttpStatusCodeResult(401, "No Username found.");
                }
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(401, "Token Null");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}