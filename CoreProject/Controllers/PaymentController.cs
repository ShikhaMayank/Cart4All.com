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
        public IActionResult Index()
        {
            ViewBag.RazorPayKey = "rzp_test_1gE6SLnkHHnitJ";//Live Key: rzp_live_ETkFO4ZYpbcxRr//testKey: rzp_test_1gE6SLnkHHnitJ
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
        
        [HttpGet]
        public JsonResult SendSuccessSMS()
        {
            ArrayList arrUserDetails = new ArrayList();
            bool isOTPSent = SendMessage.OrderConfirmation("9967248008","123456");
            return Json(isOTPSent);
        }
    }
}
