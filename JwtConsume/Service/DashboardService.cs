using JwtConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;


namespace JwtConsume.Service
{
   
    public class DashboardService : IDashboardService
    {
        private readonly string host = "https://localhost:44393";
        public async Task<List<UserInfo>> GetUsers()
        {
            //var accesstoken = new HttpContext.Session.GetString("JWTToken");

            var Userlist = new List<UserInfo>();

            using (var client = new HttpClient())

            {
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);



                string url = ($"{host}/api/Auth/show-users");
                using (var response = await client.GetAsync(url))
                {
                    var apiResponse = response.Content.ReadAsStringAsync().Result;
                    var users = JsonConvert.DeserializeObject<List<UserInfo>>(apiResponse);

                    return users;

                }
            }
        }
    }
}
