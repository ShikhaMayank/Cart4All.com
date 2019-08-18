using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using CoreProject.Models;

namespace CoreProject.Controllers
{
    public class ProductController : Controller
    {
        ExceptionLoggingToSQL objExceptionLoggingToSQL = new ExceptionLoggingToSQL();
        private IConfiguration _configuration;
        private string dbConnectionString;
        public ProductController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            dbConnectionString = _configuration["DBStrings"];
        }
        public IActionResult Index()
        {
            //if (Request.Cookies["username"] != null)
            //{
            //    //Remove("username");
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            return View();
        }
        public string Get(string key)
        {
            return Request.Cookies[key];
        }
        public void Remove(string key)
        {
            Response.Cookies.Delete(key);
        }
        // method to get menu list from database.// Currently we r bringing same from json file to increase its performance.
        [HttpPost]
        public JsonResult GetFoodItemList(string subdomain)
        {
            ArrayList arr = new ArrayList();
            try
            {
                if (subdomain != "")
                {
                    arr.Add(subdomain);
                    var FoodItems = GetFoodItemList_Admin("GetFoodItemList_Admin", arr);
                    return Json(FoodItems);
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return null;
            }
        }
        
        public string GetFoodItemList_Admin(string StoredProc, ArrayList arr)
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

        [HttpPost]
        public JsonResult updateFoodItem(string ItemId, string ItemName, string ItemPrice, string ItemStatus, string ItemType)
        {
            ArrayList arr = new ArrayList();
            try
            {
                int status;
                if (ItemId != "" && ItemId != null && ItemId != "0")
                {
                    arr.Add(Convert.ToInt32(ItemId));
                    if (ItemStatus.ToLower() == "1")
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                    arr.Add(Convert.ToBoolean(status));
                    arr.Add(Convert.ToString(ItemName));
                    arr.Add(Convert.ToInt32(ItemType));
                    arr.Add(Convert.ToString(ItemPrice));
                    var isUpdated = updateFoodItem("UpdateFoodItemStatus", arr);
                    return Json(isUpdated);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return null;
            }
        }
        public bool updateFoodItem(string StoredProc, ArrayList arr)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                StoredProc, new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Value = arr[0];

                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit);
                    cmd.Parameters["@IsActive"].Value = arr[1];

                    cmd.Parameters.Add("@name", SqlDbType.NVarChar);
                    cmd.Parameters["@name"].Value = arr[2];

                    cmd.Parameters.Add("@type", SqlDbType.Int);
                    cmd.Parameters["@type"].Value = arr[3];

                    cmd.Parameters.Add("@price", SqlDbType.Float);
                    cmd.Parameters["@price"].Value = arr[4];

                    cmd.Connection.Open();
                    DataTable table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                }
                return true;
            }
            catch(Exception ex)
            {
                objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                return false;
            }
        }
        [HttpPost]
        public bool OnOFFRestaurant(string restaurant, string status)
        {
            ArrayList arr = new ArrayList();
            try
            {
                if (restaurant != "" && restaurant != null && restaurant != "0")
                {
                    arr.Add(Convert.ToString(restaurant));
                    if (status == "true")
                    {
                        arr.Add(1);
                    }
                    else
                    {
                        arr.Add(0);
                    }
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(
                        "ONOFFRESTAURANT", new SqlConnection(dbConnectionString)))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@restaurant", SqlDbType.NVarChar);
                            cmd.Parameters["@restaurant"].Value = arr[0];
                            cmd.Parameters.Add("@status", SqlDbType.Bit);
                            cmd.Parameters["@status"].Value = arr[1];

                            cmd.Connection.Open();
                            DataTable table = new DataTable();
                            table.Load(cmd.ExecuteReader());
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        objExceptionLoggingToSQL.LogAppException(ex.StackTrace);
                        return false;
                    }
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
        public static string Between(string Text, string FirstString, string LastString)
        {
            string[] parts = Text.Split(FirstString);
            string[] parts2 = parts[1].Split(LastString);
            string myData = parts2[0];
            return myData;
        }
    }
}