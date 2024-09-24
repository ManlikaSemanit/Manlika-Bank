using ManlikaBank.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ManlikaBank.App_Start;

namespace ManlikaBank.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        [JwtAuthorization]
        public ActionResult Index()
        {
            AccountModel accountModel = new AccountModel();
            List<TransferModel> transferModel = new List<TransferModel>();
            string connectionString = ConfigurationManager.ConnectionStrings["EntityContext"].ToString();
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
                        cmd.CommandText = "Account_Sel";
                        cmd.Parameters.Add(new SqlParameter("@UserID", userID));
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet dtsResult = new DataSet();
                        adp.Fill(dtsResult);

                        DataTable dtbResult = dtsResult.Tables[0];
                        DataTable dtbResultHistory = dtsResult.Tables[1];

                        if (dtbResultHistory.Rows.Count > 0)
                        {
                            transferModel = dtbResultHistory.Rows.Count > 0 ? dtbResultHistory.AsEnumerable().Select(s => new TransferModel
                            {
                                TransactionID = s.Field<int>("TransactionID"),
                                Datetime = s.Field<DateTime?>("Datetime"),
                                ActionName = s.Field<string>("ActionName"),
                                Name = s.Field<string>("Name"),
                                Amount = s.Field<string>("Amount"),
                            }).ToList() : new List<TransferModel>();
                        }

                        if (dtbResult.Rows.Count > 0)
                        {
                            accountModel = dtbResult.Rows.Count > 0 ? dtbResult.AsEnumerable().Select(s => new AccountModel
                            {
                                BankNo = s.Field<string>("BankNo"),
                                Balance = s.Field<decimal>("Balance"),
                                TransferModels = transferModel
                            }).FirstOrDefault() : new AccountModel();
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

            return View(accountModel);
        }
    }
}