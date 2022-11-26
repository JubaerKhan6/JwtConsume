using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(String[] args)
        {
            string url = "https://localhost:44388/api/Movies/AddMovie";

            var test = new
            {
                Name = "Inception",
                Released = 2011,
                Director = "Christopher Nolan",
                Image = "Images/Inception.png"
            };

            string json = JsonConvert.SerializeObject(test, Formatting.Indented);

            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response1 = httpClient.PostAsync(url, httpContent).Result;

                var response2 = await httpClient.PostAsync(url, httpContent);

                var response3 = httpClient.PostAsync(url, httpContent);
            }

            string Result = "";
        }

        public static async Task<object> TEST()
        {
            string url = "https://localhost:44388/api/Movies/AddMovie";

            var test = new
            {
                Name = "Inception",
                Released = 2011,
                Director = "Christopher Nolan",
                Image = "Images/Inception.png"
            };

            string json = JsonConvert.SerializeObject(test, Formatting.Indented);

            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                return httpClient.PostAsync(url, httpContent);
            }
        }
    }
}