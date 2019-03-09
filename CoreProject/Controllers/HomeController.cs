using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreProject.Models;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using CoreProject.Models.db;
using System.Data.Entity.Core.EntityClient;
using System.Data;
using System.Data.SqlClient;

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
        }
        // GET: MenuMaster
        public async Task<IActionResult> GetMenu()
        {            
            return Json(await _context.Menumaster.ToListAsync());
        }

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


        [HttpPost]
        public ActionResult GetFoodItemList(string MenuId)
        {
            ArrayList arr = new ArrayList();
            arr.Add(1);
            var menuItems = GetMenuList("GetMenuById", arr);
            return Json(menuItems);
        }
        public IActionResult Index()
        {            
            return View();
        }
        // GET: /Home/LoadJson
        [HttpGet]
        public JsonResult LoadJson()
        {
            ArrayList arrPageDetails = new ArrayList();
            arrPageDetails.Add(" House No. 41, Meadow Greens, Lodha Heaven, Dombivali Palava City, Maharashtra 421204");
            arrPageDetails.Add("Snacks");
            arrPageDetails.Add("+91-81699 05387");
            return Json(arrPageDetails);
        }
        
        [HttpGet]
        public JsonResult GetOTP()
        {            
            ArrayList arrUserDetails = new ArrayList();
            Random generator = new Random();
            string OTP = generator.Next(0, 999999).ToString("D6");
            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey, publicKey);
            bool isOTPSent = SendSMS(OTP);
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
        public bool SendSMS(string OTP)
        {
            string sURL = smsUrl + smsKey + "&senderid=" + senderId + "&route=" + route + "&number=" + number + "&message=" + message + OTP;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString(sURL);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
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
