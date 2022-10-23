using JwtConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace JwtConsume.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string host = "https://localhost:44393";
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetName()
        {
            var accessToken = HttpContext.Session.GetString("JWTToken");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                string url = ($"{host}/api/Auth/See-your-username");

                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    
                    
                    TempData["message"] = apiResponse;

                    
                    
                };
            };
            return View();
            
        }
    }
}
