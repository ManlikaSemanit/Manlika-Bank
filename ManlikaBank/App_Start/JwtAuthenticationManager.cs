using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using System.Linq;
using ManlikaBank.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Web;


namespace Manlika_WebApi.App_Start
{
    public class JwtAuthenticationManager
    {
        public static string GenerateJWTAuthetication(string userName, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtHeaderParameterNames.Jku, userName),
                new Claim(JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userName)
            };


            claims.Add(new Claim(ClaimTypes.Role, role));


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =
                DateTime.Now.AddDays(
                    Convert.ToDouble(Convert.ToString(ConfigurationManager.AppSettings["config:JwtExpireDays"])));

            var token = new JwtSecurityToken(
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                var jwtToken = (JwtSecurityToken)validatedToken;
                var jku = jwtToken.Claims.First(claim => claim.Type == "jku").Value;
                var userName = jwtToken.Claims.First(claim => claim.Type == "kid").Value;

                return userName;
            }
            catch
            {
                return null;
            }
        }

        public static LoginModel FindUser(string username, string password)
        {
            //TransferModel testModel = new TransferModel();
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
    }
}
