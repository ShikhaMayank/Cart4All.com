using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreProject.Models;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CoreProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly mayankdbContext _context;

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

        public HomeController(mayankdbContext context, IConfiguration Configuration)
        {
            _context = context;
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
            //Subject = "This is test mail using smtp settings";
            //Body = "SendGrid Mail";
            //ToEmail = "mayank.gpt1@gmail.com";
            //SMTPUser = "mybusinesscart@gmail.com";
            //SMTPPassword = "Welcome@123456";
        }
        // GET: MenuMaster
        public async Task<IActionResult> GetMenu()
        {
            //dynamic arr = JsonConvert.SerializeObject(await _context.Menumaster.ToListAsync()); // to get menu list into json object
            return Json(await _context.Menumaster.ToListAsync());
        }

        // method to get menu list from database.// Currently we r bringing same from json file instead of hitting this below method to increase performance of site.
        public string GetMenuList(string StoredProc, ArrayList arr)
        {
            string JSONresult;
            using (SqlCommand cmd = new SqlCommand(
                StoredProc, new SqlConnection(dbConnectionString)))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@RestaurantId", SqlDbType.Int);
                cmd.Parameters["@RestaurantId"].Value = arr[0];
                cmd.Connection.Open();
                DataTable table = new DataTable();
                table.Load(cmd.ExecuteReader());
                JSONresult = JsonConvert.SerializeObject(table);
            }
            return JSONresult;
        }

        // method to get menu list from database.// Currently we r bringing same from json file to increase its performance.
        [HttpPost]
        public ActionResult GetFoodItemList(string MenuId)
        {
            ArrayList arr = new ArrayList();
            arr.Add(MenuId);
            try
            {
                var menuItems = GetMenuList("GetMenuById", arr);
                return Json(menuItems);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public string GetRestaurantDetails(string StoredProc, string domainName)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    StoredProc, new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@domainName", SqlDbType.VarChar);
                    cmd.Parameters["@domainName"].Value = domainName;
                    cmd.Connection.Open();
                    DataTable table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    return JsonConvert.SerializeObject(table);
                }
            }
            catch
            {
                return null;
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: /Home/LoadJson
        [HttpPost]
        public JsonResult LoadJson(string domain)
        {
            var details = GetRestaurantDetails("GetRestaurantDetails", domain);
            return Json(details);
        }

        [HttpGet]
        public JsonResult GetOTP()
        {
            ArrayList arrUserDetails = new ArrayList();
            Random generator = new Random();
            string OTP = generator.Next(0, 999999).ToString("D6");
            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);
            string Message = smsUrl + smsKey + "&senderid=" + senderId + "&route=" + route + "&number=" + number + "&message=" + message + OTP;
            bool isOTPSent = SendMessage.SendSMS(OTP, Message);
            //bool isOTPSent = true;
            if (isOTPSent == true)
            {
                arrUserDetails.Add(rsa.Encrypt(OTP));
            }
            else
            {
                arrUserDetails.Add("SMS Not Sent");
            }
            return Json(arrUserDetails);
        }
        string OrderId = "";
        [HttpPost]
        public ActionResult VerifyOTP(string OTP, string HashCode)
        {
            ArrayList arrStatus = new ArrayList();
            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);

            Enduser VerifyOTP = new Enduser
            {
                OTP = rsa.Decrypt(HashCode)
            };
            if (VerifyOTP.OTP == OTP)
            {
                // Insertion to tables and generation of order id code will go here..
                OrderId = GetOrderId();
                return Json(OrderId);
            }
            else
            {
                return Json("Error");
            }

        }
        public string GetOrderId()
        {
            Random generator = new Random();
            return Convert.ToString(generator.Next(0, 999999).ToString("D6"));
        }
        public class Enduser
        {
            public string Name
            {
                get;
                set;
            }
            public string Phone
            {
                get;
                set;
            }
            public string HashCode
            {
                get;
                set;
            }
            public string OTP
            {
                get;
                set;
            }
            public bool SMSStatus
            {
                get;
                set;
            }
        }
        public ActionResult FoodItems()
        {
            return PartialView();
        }
        #region Other pages code
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Thanks()
        {
            return View();
        }
        public IActionResult error()
        {
            return View();
        }
        public IActionResult Partner()
        {
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion
    }
}
