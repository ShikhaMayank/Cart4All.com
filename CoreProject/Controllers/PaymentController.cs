using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreProject.Models;
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
        public JsonResult SendSuccessSMS(string phone, string orderId, string paymentId)
        {
            ArrayList arrUserDetails = new ArrayList();
            bool isOTPSent = SendMessage.OrderConfirmationSMS(phone, orderId, paymentId);
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
                if (paymentId != "" || paymentId != null)
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
                isOTPSent = false;
            }
            
            return Json(isOTPSent);
        }
    }
}
