using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using CoreProject.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace CoreProject.Controllers
{
    public class PaymentController : Controller
    {
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
        private string RazorPayKey;
        ExceptionLoggingToSQL objExceptionLoggingToSQL = new ExceptionLoggingToSQL();
        public PaymentController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            publicKey = _configuration["PublicKey"];
            privateKey = _configuration["PrivateKey"];
            smsUrl = _configuration["SmsUrl"];
            smsKey = _configuration["SmsKey"];
            senderId = _configuration["SenderId"];
            route = _configuration["Route"];
            number = _configuration["Number"];
            message = _configuration["Message"];
            SMTPPasswordAPI = _configuration["SMTPPasswordAPI"];
            RazorPayKey = _configuration["RazorPayKey"];
            dbConnectionString = _configuration["DBStrings"];
            //Subject = "This is test mail using smtp settings";
            //Body = "SendGrid Mail";
            //ToEmail = "mayank.gpt1@gmail.com";
            //SMTPUser = "mybusinesscart@gmail.com";
            //SMTPPassword = "Welcome@123456";
        }
        public IActionResult Index()
        {
            ViewBag.RazorPayKey = RazorPayKey;//Live Key: rzp_live_ETkFO4ZYpbcxRr//testKey: rzp_test_1gE6SLnkHHnitJ
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult SendSuccessSMS(string phone, string orderId, string orderedItems, string paymentId)
        {
            ArrayList arrUserDetails = new ArrayList();
            bool isOTPSent = SendMessage.OrderConfirmationSMS(phone, orderId, orderedItems, paymentId);
            return Json(isOTPSent);
        }
        [HttpPost]
        public JsonResult SendMailToOwner(string userDetails, string toEmail, string orderId, string paymentId)
        {
            bool isOTPSent = false;
            try
            {
                SendMail obj = new SendMail();
                Credentials objCred = new Credentials();
                objCred.SMTPUser = "mybusinesscart@gmail.com";
                objCred.SMTPPassword = "Welcome@123456";
                objCred.Host = "smtp.gmail.com";
                objCred.Port = 25;
                if (paymentId != "" && paymentId != null)
                {
                    objCred.Subject = "New order received with Order Id: " + orderId + " and payment Id: " + paymentId;
                }
                else
                {
                    objCred.Subject = "New COD order received with Order Id: " + orderId;
                }

                objCred.IsBodyHtml = true;
                objCred.EnableSsl = true;
                objCred.Body = "<html><body><div>" + userDetails.Trim() + "</div></body></html>";
                
                
                if (toEmail == "" || toEmail == null)
                {
                    isOTPSent = false;
                }
                else
                {
                    objCred.ToEmail = toEmail;
                    isOTPSent = obj.Gmail(objCred);
                    isOTPSent = true;
                }
            }
            catch(Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                isOTPSent = false;
            }
            
            return Json(isOTPSent);
        }

        [HttpPost]
        public bool CreateOrder(string orderId,string OrderedItems, string razorpay_payment_id, string name, string email, string mobile, string price, string address, string restaurant)
        {
            try
            {
                // Insertion to tables and generation of order id code will go here..
                var OrderId = CreateNewOrderPayment(orderId, OrderedItems, razorpay_payment_id, name, email, mobile, price, address, restaurant);// delivery mode is 1(COD) and paymentID is blank
                if (OrderId != Guid.Empty)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return false;
            }
        }
        public Guid CreateNewOrderPayment(string _orderId, string _orderedItems, string razorpay_payment_id, string name, string email, string mobile, string price, string address, string restaurant)
        {
            try
            {
                var orderId = new Guid(_orderId);
                using (SqlCommand cmd = new SqlCommand(
                "CreateNewOrderPayment", new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@OrderId", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@OrderId"].Value = orderId;

                    cmd.Parameters.Add("@OrderedItems", SqlDbType.NVarChar);
                    cmd.Parameters["@OrderedItems"].Value = _orderedItems;

                    cmd.Parameters.Add("@username", SqlDbType.VarChar);
                    cmd.Parameters["@username"].Value = name;

                    cmd.Parameters.Add("@userPhone", SqlDbType.VarChar);
                    cmd.Parameters["@userPhone"].Value = mobile;

                    cmd.Parameters.Add("@userEmail", SqlDbType.VarChar);
                    cmd.Parameters["@userEmail"].Value = email;

                    cmd.Parameters.Add("@address", SqlDbType.NVarChar);
                    cmd.Parameters["@address"].Value = address;

                    cmd.Parameters.Add("@price", SqlDbType.Float);
                    cmd.Parameters["@price"].Value = price;

                    cmd.Parameters.Add("@deliveryMode", SqlDbType.Int);
                    cmd.Parameters["@deliveryMode"].Value = 2;

                    cmd.Parameters.Add("@paymentId", SqlDbType.NVarChar);
                    cmd.Parameters["@paymentId"].Value = razorpay_payment_id;

                    cmd.Parameters.Add("@restaurant", SqlDbType.NVarChar);
                    cmd.Parameters["@restaurant"].Value = restaurant;

                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                }
                return orderId;
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return Guid.Empty;
            }
        }
    }
}
