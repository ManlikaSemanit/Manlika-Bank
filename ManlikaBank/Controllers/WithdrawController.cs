using ManlikaBank.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManlikaBank.App_Start;

namespace ManlikaBank.Controllers
{
    public class WithdrawController : BaseController
    {
        // GET: Withdraw
        [JwtAuthorization]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult WithdrawMoney(decimal amount)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EntityContext"].ToString();
            ReturnModel returnData = new ReturnModel();
            int userID = int.Parse(CurrentUserID());
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
                        cmd.CommandText = "WithdrawMoney_Ins";
                        cmd.Parameters.Add(new SqlParameter("@UserID", userID));
                        cmd.Parameters.Add(new SqlParameter("@Amount", amount));
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet dtsResult = new DataSet();
                        adp.Fill(dtsResult);

                        DataTable dtbResult = dtsResult.Tables[0];

                        if (dtbResult.Rows.Count > 0)
                        {
                            returnData = dtbResult.Rows.Count > 0 ? dtbResult.AsEnumerable().Select(s => new ReturnModel
                            {
                                IsSuccessful = s.Field<bool>("IsSuccessful"),
                                ErrorMessage = s.Field<string>("ErrorMessage"),
                            }).FirstOrDefault() : new ReturnModel();
                        }
                    }
                }
                catch (Exception ex)
                {
                    returnData.IsSuccessful = false;
                    returnData.ErrorMessage = ex.ToString();
                }
                finally
                {
                    xSqlCon.Close();
                }
            }

            var resultList = new
            {
                IsSuccessful = returnData.IsSuccessful,
                ErrorMessage = returnData.ErrorMessage
            };

            JsonResult jsonResult = Json(resultList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
    }
}