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
                return false;
            }
        }        
        public static bool OrderConfirmationSMS(string phone, string orderId, string orderedItems, string paymentId)
        {
            smsKey = "36306361727434616c6c2e636f6d3734351549362176";// SMS API Key
            string msg = "";
            if (paymentId == null || paymentId.Trim() == "" || paymentId == string.Empty) {
                msg = "Dear user, your ordered items with price and count are: "+ orderedItems + "\nPlease use Order ID " + orderId +" for all your future reference.";
            }
            else
            {
                msg = "Dear user, your Order is confirmed with ID: " + orderId + " and transaction id: " + paymentId + ". Please use them for your future reference";
            }
            
            string Message = "http://text.bluemedia.in/http-tokenkeyapi.php?authentic-key=" + smsKey + "&senderid=" + "MYCART" + "&route=" + "2" + "&number=" + phone + "&message=" + msg;
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
                return false;
            }
        }
    }
}
