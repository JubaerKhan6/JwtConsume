using JwtConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace JwtConsume.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string host = "https://localhost:44393";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoginUser(User user)
        {
            using (var client = new HttpClient())
            {
                
                string jsonData = JsonConvert.SerializeObject(user);
                StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                string url = $"{host}/api/Auth/login";

               

                using (var response = await client.PostAsync(url, stringContent))

                {
                    string token = await response.Content.ReadAsStringAsync();
                    bool compare = token.Contains("One or more validation errors occurred.");

                    if (compare)
                    {
                        TempData["message"] = "Please fill out all the required fields.";
                        return Redirect("~/Home/Index");
                    }
                    else if (token == "User not found")
                    {
                        TempData["message"] = "The User Does not exist";

                        return Redirect("~/Home/Index");
                    }
                    else if (token == "Wrong Password")
                    {
                        TempData["message"] = "Wrong Password";
                        return Redirect("~/Home/Index");
                    }
                    HttpContext.Session.SetString("JWTToken", token);
                }
                return Redirect("~/Dashboard/Index");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Registration(Reg register)
        {
            using (var client = new HttpClient())
            {
                string jsonData = JsonConvert.SerializeObject(register);
                StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                string url = $"{host}/api/Auth/register";

                using (var response = await client.PostAsync(url, stringContent))

                {
                    string token = await response.Content.ReadAsStringAsync();
                    bool compare = token.Contains("One or more validation errors occurred.");

                    if (compare)
                    {
                        TempData["message"] = "Please fill out all the required fields.";
                        return Redirect("~/Home/register");
                    }

                    TempData["message"] = token;
                }
                return Redirect("~/Home/Register");
            }

        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}