using JwtConsume.Models;
using JwtConsume.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using NuGet.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace JwtConsume.Controllers
{
   
    public class DashboardController : Controller
    {
        private readonly string host = "https://localhost:44393";
        private readonly string host2 = "https://localhost:44388";
        public string? accesstoken;

        //public IActionResult tokenstore()
        //{
        //    accesstoken = HttpContext.Session.GetString("JWTToken");
        //    return RedirectToAction("Index");
        //}
       
        
        public IActionResult Index()
        {
            
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            return View();
        }

      
        public async Task<IActionResult> Logout()
        {
            var token = "Not";
            HttpContext.Session.SetString("JWTToken", token);

           
            return Redirect("~/Home");
            
           
        }
        public async Task<ActionResult> GetDetails()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url1 = ($"{host}/api/Auth/See-your-username");
                string url2 = ($"{host}/api/Auth/show-roles");
                string url3 = ($"{host}/api/Auth/show-Id");

                using (var response = await client.GetAsync(url1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();



                    TempData["message"] = apiResponse;



                };
                using (var response2 = await client.GetAsync(url2))
                {
                    string apiResponse2 = await response2.Content.ReadAsStringAsync();



                    TempData["role"] = apiResponse2;



                };
                using (var response3 = await client.GetAsync(url3))
                {
                    string apiResponse3 = await response3.Content.ReadAsStringAsync();

                    TempData["Id"] = apiResponse3;
                }
            };
            return View();

        }
        public async Task<ActionResult> GetRole()
        {

            var accessToken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                string url2 = ($"{host}/api/Auth/show-roles");

                using (var response2 = await client.GetAsync(url2))
                {
                    string apiResponse2 = await response2.Content.ReadAsStringAsync();



                    TempData["role"] = apiResponse2;



                };
            };
            return View();

        }
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> GetUsers()
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            var Userlist = new List<UserInfo>();

            using (var client = new HttpClient())

            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/Auth/show-users");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    Userlist = JsonConvert.DeserializeObject<List<UserInfo>>(readtask);
                    return View(Userlist);
                }
            }
            return RedirectToAction("AccessDenied");
            
        }
        public async Task<IActionResult> GetTokens()
        {

            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            var tokenlist = new List<RefreshTokens>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/Auth/show-refreshTokens");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    tokenlist = JsonConvert.DeserializeObject<List<RefreshTokens>>(readtask);
                    return View(tokenlist);
                }
            }
            return Ok("Access Denied");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                
                string url =($"{host}/api/Auth/get-by-{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse= await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UserInfo>(apiResponse);
                    return View(result);
                }


            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            using (var client = new HttpClient())
            {
               
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/Auth/edit-user");
                using (var response = await client.PostAsJsonAsync(url, user))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    
                }
            }
            return RedirectToAction("GetUsers");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host}/api/Auth/delete/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    
                    return RedirectToAction("GetUsers");
                }
                return RedirectToAction("GetUsers");

            }
        }

        public async void ClearCart()
        {


            using (var client = new HttpClient())
            {
                string url = ($"{host2}/api/Cart/DeleteCart");

                var c = await client.DeleteAsync(url);

                var apiresponse = c.Content.ReadAsStringAsync();

            }
        }

        public async Task<IActionResult> OrderHistory(int id)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not") { return Redirect("~/Home"); }

            string url = $"{host2}/api/Orders/GetOrderbyUser/{id}";

            using(var client = new HttpClient())
            {
                var c = await client.GetAsync(url);

                var read = await c.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Orders>>(read);
                return View(result);

            }
        }


    }
}
