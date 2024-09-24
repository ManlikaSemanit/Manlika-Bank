using ManlikaBank.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Manlika_WebApi.App_Start;

namespace ManlikaBank.Controllers
{
    public class BaseController : Controller
    {
        //private JwtAuthenticationManager jwtAuthenticationManager;
        public const string SESSION_USERNAME = "Username";
        public const string SESSION_USERID = "UserID";
        public const string SESSION_USERINFO = "UserInfo";
        public string CurrentUserID()
        {
            string userId = Session[SESSION_USERID] as string;
            if (string.IsNullOrEmpty(userId))
            {
                LoginModel userInfo = CurrentUserInfo();
                userId = "N/A";
                if (userInfo != null)
                {
                    userId = userInfo.UserID.ToString();
                }
            }
            return userId;
        }

        public LoginModel CurrentUserInfo()
        {

            LoginModel userInfo = (LoginModel)Session[SESSION_USERINFO];
            if (userInfo == null)
            {
                userInfo = FindUser(CurrentUsername());
                Session[SESSION_USERINFO] = userInfo;
            }
            return userInfo;
        }

        protected string CurrentUsername()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                userName = Session[SESSION_USERNAME] as string;

            return userName?.ToLower();
        }

        public LoginModel FindUser(string username, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EntityContext"].ToString();
            LoginModel returnData = new LoginModel();

            using (SqlConnection xSqlCon = new SqlConnection(connectionString))
            {
                try
                {
                    xSqlCon.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = xSqlCon;
                        cmd.Parameters.Clear();
                        cmd.CommandTimeout = 1200;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "User_Sel";
                        cmd.Parameters.Add(new SqlParameter("@Username", username));
                        cmd.Parameters.Add(new SqlParameter("@Password", password));
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet dtsResult = new DataSet();
                        adp.Fill(dtsResult);

                        DataTable dtbResult = dtsResult.Tables[0];

                        if (dtbResult.Rows.Count > 0)
                        {
                            returnData = dtbResult.Rows.Count > 0 ? dtbResult.AsEnumerable().Select(s => new LoginModel
                            {
                                UserID = s.Field<int>("UserID"),
                                UserName = s.Field<string>("UserName"),
                                Password = s.Field<string>("Password"),
                            }).FirstOrDefault() : new LoginModel();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    xSqlCon.Close();
                }
            }

            return returnData;
        }

        public LoginModel FindUser(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EntityContext"].ToString();
            LoginModel returnData = new LoginModel();

            using (SqlConnection xSqlCon = new SqlConnection(connectionString))
            {
                try
                {
                    xSqlCon.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = xSqlCon;
                        cmd.Parameters.Clear();
                        cmd.CommandTimeout = 1200;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "UserByUsername_Sel";
                        cmd.Parameters.Add(new SqlParameter("@Username", username));
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet dtsResult = new DataSet();
                        adp.Fill(dtsResult);

                        DataTable dtbResult = dtsResult.Tables[0];

                        if (dtbResult.Rows.Count > 0)
                        {
                            returnData = dtbResult.Rows.Count > 0 ? dtbResult.AsEnumerable().Select(s => new LoginModel
                            {
                                UserName = s.Field<string>("UserName"),
                                UserID = s.Field<int>("UserID"),
                            }).FirstOrDefault() : new LoginModel();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    xSqlCon.Close();
                }
            }

            return returnData;
        }

        //public JwtAuthenticationManager(string key)
        //{
        //    this.key = key;
        //}
        //public string Authenticate(string username, string password)
        //{
        //    //auth failed - creds incorrect
        //    var users = FindUser(username, password);
        //    if (users == null)
        //    {
        //        return null;
        //    }

        //    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenKey = Encoding.ASCII.GetBytes(key);
        //    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, username)
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(2),
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(tokenKey),
        //            SecurityAlgorithms.HmacSha256Signature) //setting sha256 algorithm
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}