using System;
using System.Net;
using Microsoft.Extensions.Configuration;
namespace CoreProject.Models
{
    public static class SendMessage
    {
        static string smsUrl;
        static string smsKey;
        static string senderId;
        static string route;
        private static IConfiguration _configuration;

        public static bool SendSMS(string OTP, string Message)
        {            
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString(Message);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
        public static bool OrderConfirmation(string number, string orderID)
        {

            smsKey = "36306361727434616c6c2e636f6d3734351549362176";
            string msg = "Dear user, your Order is confirmed with order ID " + orderID;
            string Message = "http://text.bluemedia.in/http-tokenkeyapi.php?authentic-key=" + smsKey + "&senderid=" + "MYCART" + "&route=" + "2" + "&number=" + number + "&message=" + msg;
            try
            {
                using (WebClient client = new WebClient())
                {
                    string s = client.DownloadString(Message);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }
    }
}
