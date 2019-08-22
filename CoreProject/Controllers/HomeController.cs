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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace CoreProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly mayankdbContext _context;
        private ILogger logger = null;
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
        ExceptionLoggingToSQL objExceptionLoggingToSQL = new ExceptionLoggingToSQL();

        public HomeController(mayankdbContext context, IConfiguration Configuration, ILogger<DebugLogger> logger)
        {
            _context = context;
            _configuration = Configuration;
            this.logger = logger;
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
                cmd.Parameters.Add("@domain", SqlDbType.NVarChar);
                cmd.Parameters["@domain"].Value = arr[0];
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
                var FoodItems = GetMenuList("GetFoodItemList", arr);
                return Json(FoodItems);
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
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
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return null;
            }
        }
        public IActionResult Index()
        {
            logger.LogInformation("This is a log message!");
            logger.LogInformation("exception message", "This is a log message!");
            return View();
        }
        // GET: /Home/LoadJson
        [HttpPost]
        public JsonResult LoadJson(string domain)
        {
            var details = GetRestaurantDetails("GetRestaurantDetails", domain);
            return Json(details);
        }

        [HttpPost]
        public JsonResult GetOTP(string number)
        {
            ArrayList arrUserDetails = new ArrayList();
            try
            {
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
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                arrUserDetails.Add("ERROR");
            }

            return Json(arrUserDetails);
        }

        [HttpGet]
        public JsonResult SendOrderId(string number, string myOrderId,string orderedItems, string userName, string paymentId)
        {
            bool isOTPSent = true;
            try
            {
                if (paymentId == null || paymentId.Trim() == "" || paymentId == string.Empty)
                {
                    paymentId = "";
                }
                //string Message = smsUrl + smsKey + "&senderid=" + senderId + "&route=" + route + "&number=" + number + "&message=Dear "+ userName + ", your Order confimed with order Id " + myOrderId + ". We will be contacting you as soon possible regarding your placed order.";
                isOTPSent = SendMessage.OrderConfirmationSMS(number, myOrderId, orderedItems, paymentId);
                isOTPSent = true;
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                isOTPSent = false;
            }
            return Json(isOTPSent);
        }

        [HttpPost]
        public ActionResult VerifyOTP(string OTP, string HashCode, string OrderedItems, string username, string userPhone, string userEmail, string address, string restaurant)
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
                var OrderId = CreateNewOrder(OrderedItems, username, userPhone, userEmail, address, 1,"", restaurant);// delivery mode is 1(COD) and paymentID is blank
                if(OrderId != Guid.Empty)
                {
                    return Json(OrderId);
                }
                else
                {
                    return Json("00000000-0000-0000-0000-000000000000");
                }
            }
            else
            {
                return Json("Error");
            }

        }

        public Guid CreateNewOrder(string OrderedItems, string username, string userPhone, string userEmail, string address, int deliveryMode, string paymentId, string restaurant)
        {            
            try
            {
                var OrderId = Guid.NewGuid();
                using (SqlCommand cmd = new SqlCommand(
                "CreateNewOrder", new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@OrderId", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@OrderId"].Value = OrderId;

                    cmd.Parameters.Add("@OrderedItems", SqlDbType.VarChar);
                    cmd.Parameters["@OrderedItems"].Value = OrderedItems;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar);
                    cmd.Parameters["@username"].Value = username;

                    cmd.Parameters.Add("@userPhone", SqlDbType.VarChar);
                    cmd.Parameters["@userPhone"].Value = userPhone;

                    cmd.Parameters.Add("@userEmail", SqlDbType.VarChar);
                    cmd.Parameters["@userEmail"].Value = userEmail;

                    cmd.Parameters.Add("@address", SqlDbType.NVarChar);
                    cmd.Parameters["@address"].Value = address;

                    cmd.Parameters.Add("@deliveryMode", SqlDbType.Int);
                    cmd.Parameters["@deliveryMode"].Value = deliveryMode;

                    cmd.Parameters.Add("@paymentId", SqlDbType.NVarChar);
                    cmd.Parameters["@paymentId"].Value = paymentId;

                    cmd.Parameters.Add("@restaurant", SqlDbType.NVarChar);
                    cmd.Parameters["@restaurant"].Value = restaurant;

                    cmd.Connection.Open();                    
                    cmd.ExecuteReader();
                }
                return OrderId;
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return Guid.Empty;
            }            
        }
        public static string GetOrderId()
        {
            var objOrderId = Guid.NewGuid();
            return Convert.ToString(objOrderId);
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
        public IActionResult ErrorPage()
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
