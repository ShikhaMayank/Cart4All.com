using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace CoreProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        private string dbConnectionString;
        private string publicKey;
        private string privateKey;
        private string smsUrl;
        private string smsKey;
        private string senderId;
        private string route;
        private string number;
        private string message;
        private string SMTPPasswordAPI;
        public LoginController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            dbConnectionString = _configuration["DBStrings"];
            publicKey = _configuration["PublicKey"];
            privateKey = _configuration["PrivateKey"];
            smsUrl = _configuration["SmsUrl"];
            smsKey = _configuration["SmsKey"];
            senderId = _configuration["SenderId"];
            route = _configuration["Route"];
            number = _configuration["Number"];
            message = _configuration["Message"];
            SMTPPasswordAPI = _configuration["SMTPPasswordAPI"];
        }        
        public void Set(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            Response.Cookies.Append(key, value, option);
        }        
        public IActionResult Index()
        {            
            return View();
        }
        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            dynamic JSONresult;
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "pr_Authenticate", new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar);
                    cmd.Parameters["@username"].Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar);
                    cmd.Parameters["@password"].Value = password;
                    cmd.Connection.Open();
                    dynamic table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    JSONresult = JsonConvert.SerializeObject(table);

                    TempData["username"] = username;
                    //set the key value in Cookie  
                    Set("username", username, 10);
                    ////Delete the cookie object  
                    //Remove("username");
                    return Json(JSONresult);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}