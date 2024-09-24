using Manlika_WebApi.App_Start;
using ManlikaBank.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Security;
using System.Net;
//using StructureMap;

namespace ManlikaBank.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Signin(string username, string password)
        {
            try
            {
                if (username != null && password != null)
                {
                    var users = FindUser(username, password);
                    if (users.UserID > 0)
                    {
                        var role = users.UserName;
                        var jwtToken = JwtAuthenticationManager.GenerateJWTAuthetication(username, role);
                        var validUserName = JwtAuthenticationManager.ValidateToken(jwtToken);

                        if (string.IsNullOrEmpty(validUserName))
                        {
                            var resultList = new
                            {
                                IsSuccessful = false,
                                ErrorMessage = "Unauthorized login attempt"
                            };

                            JsonResult jsonResult = Json(resultList, JsonRequestBehavior.AllowGet);
                            jsonResult.MaxJsonLength = int.MaxValue;

                            return jsonResult;
                        }

                        var cookie = new HttpCookie("jwt", jwtToken)
                        {
                            HttpOnly = true,
                        };
                        Response.Cookies.Add(cookie);

                        Session[SESSION_USERINFO] = users;

                        ViewBag.JwtToken = jwtToken;

                    }
                    else
                    {
                        var resultList = new
                        {
                            IsSuccessful = false,
                            ErrorMessage = "Username Or Password Invalid."
                        };

                        JsonResult jsonResult = Json(resultList, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;

                        return jsonResult;
                    }
                    
                }
                else
                {
                    var resultList = new
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Username Or Password Invalid."
                    };

                    JsonResult jsonResult = Json(resultList, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;

                    return jsonResult;
                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }

            var resultList1 = new
            {
                IsSuccessful = true,
                ErrorMessage = ""
            };

            JsonResult jsonResult1 = Json(resultList1, JsonRequestBehavior.AllowGet);
            jsonResult1.MaxJsonLength = int.MaxValue;

            return jsonResult1;
        }

        public ActionResult LogOff()
        {
            if (Request.Cookies["jwt"] != null)
            {
                Response.Cookies["jwt"].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

    }
}