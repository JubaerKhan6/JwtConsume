
using JwtConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace JwtConsume.Controllers
{
    public class StoreController : Controller
    {
        private readonly string host = "https://localhost:44388";
        private readonly string host2 = "https://localhost:44388";
        private readonly string host3 = "https://localhost:44393";
        private readonly string idget = "https://localhost:44393/api/Auth/show-Id";

        private string? accesstoken;
        private readonly IWebHostEnvironment _hostenvironment;
        public StoreController(IWebHostEnvironment hostEnvironment)
        {
            _hostenvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            string url = $"{host3}/api/Auth/show-roles";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url))
                {

                
                    string role = await response.Content.ReadAsStringAsync();

                    bool compare = role.Contains("Admin");
                    if (compare)
                    {
                        return View();
                    }

                    return Redirect("~/Dashboard/AccessDenied");
                }
               
            }


        }

        public async Task<IActionResult> Index2()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            Index2Vm model = new Index2Vm();
           
            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host2}/api/Movies/GetMovies");
                string url2 = ($"{host2}/api/VideoGames/GetGames");
                string url3 = ($"{host2}/api/Cart/GetAllProducts");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    var movielist = JsonConvert.DeserializeObject<List<Movieget>>(readtask);
                    model.Movies = movielist;
                }

                var response2 = await client.GetAsync(url2);

                if (response2.IsSuccessStatusCode)
                {
                    var readtask = response2.Content.ReadAsStringAsync().Result;
                    var gamelist = JsonConvert.DeserializeObject<List<VideoGames>>(readtask);
                    model.VideoGames = gamelist;
                }
                var response3 = await client.GetAsync(url3);

                if (response2.IsSuccessStatusCode)
                {
                    var readtask = response2.Content.ReadAsStringAsync().Result;
                    var gamelist = JsonConvert.DeserializeObject<List<Cart>>(readtask);
                    model.Carts = gamelist;
                }

            }
            return View(model);

            
        }

        public async Task<IActionResult> GetHeroes()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            var superlist = new List<Superheroes>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = ($"{host}/api/SuperHero/GetHeroes");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    superlist = JsonConvert.DeserializeObject<List<Superheroes>>(readtask);
                    return View(superlist);
                }
            }
            return View(superlist);
        }

        public async Task<IActionResult> GetGames()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var gamelist = new List<VideoGames>();

            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/VideoGames/GetGames");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    gamelist = JsonConvert.DeserializeObject<List<VideoGames>>(readtask);
                    return View(gamelist);
                }

            }
            return View(gamelist);
        }
        [HttpGet]
        public async Task<IActionResult> EditGames(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);


                string url = ($"{host}/api/VideoGames/GetGames/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<VideoGameEdit>(apiResponse);
                    return View(result);
                }


            }
        }
        [HttpPost]
        public async Task<IActionResult> EditGames(VideoGameEdit vg)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }


            using (var client = new HttpClient())
            {
                if (vg.ImageFile == null)
                {
                    await EditGamesImg(vg.Name, vg.Developer, vg.Released, vg.Image, vg.Id, vg.Description, vg.Rating, vg.size, vg.Genre);

                }
                else
                {
                    Random rnd = new Random();
                    string randNo = rnd.Next(10000000, 99999999).ToString();

                    string img = vg.ImageFile.FileName;

                    var path = _hostenvironment.WebRootPath;
                    var file = "\\lib\\Images\\" + img;
                    var full = path + file;
                    string ext = Path.GetExtension(full);
                    string image = randNo + ext;
                    var filepath = "\\lib\\Images\\" + image;
                    var fullPath = path + filepath;



                    UploadFile(vg.ImageFile, fullPath);

                    await EditGamesImg(vg.Name, vg.Developer, vg.Released, image, vg.Id, vg.Description, vg.Rating, vg.size, vg.Genre);
                }

                

                return RedirectToAction("GetGames");
            }
        }
            public async Task<IActionResult> EditGamesImg(string name, string dev, int released, string img, int id, string description, float rating, int size, string genre )
            {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }


            var vg = new VideoGames
            {
                Name=name,
                Developer=dev,
                Released= released,
                Image=img,
                Id=id,
                Description= description,
                Rating= rating,
                size=size,
                Genre=genre
            };
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/VideoGames/EditGame");
                using (var response = await client.PutAsJsonAsync(url, vg))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                }
            }
            return RedirectToAction("GetGames");
            }
        [HttpGet]
        public async Task<IActionResult> EditHeroes(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);


                string url = ($"{host}/api/SuperHero/GetHeroes/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SuperheroesEdit>(apiResponse);
                    return View(result);
                }


            }
        }
        [HttpPost]

        public async Task<IActionResult> EditHeroes(SuperheroesEdit sh)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            if (sh.ImageFile == null)
            {
                await EditHeroesImg(sh.Id, sh.Name, sh.FirstName, sh.LastName, sh.Powers, sh.Description, sh.Image);
            }
            else
            {
                Random rnd = new Random();
                var random = rnd.Next(1000000, 99999999).ToString();
                var img = sh.ImageFile.FileName;
                var path = _hostenvironment.WebRootPath;
                var file = "\\lib\\Images\\Supe\\" + img;
                var full = path + file;
                var ext = Path.GetExtension(full);
                var image = random + ext;
                var filePath = "\\lib\\Images\\Supe\\" + image;
                var fullPath = path + filePath;

                UploadFile(sh.ImageFile, fullPath);
                await EditHeroesImg(sh.Id, sh.Name, sh.FirstName, sh.LastName, sh.Powers, sh.Description, image);

            }
            

            return RedirectToAction("GetHeroes");


        }

            public async Task<IActionResult> EditHeroesImg(int id, string name, string firstname, string lastname, string powers, string description, string image)
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                var sh = new Superheroes
                {
                    Id=id,
                    Name=name,
                    FirstName=firstname,
                    LastName=lastname,
                    Powers=powers,
                    Description=description,
                    Image=image

                };
                    using (var client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                        string url = ($"{host}/api/Superhero/EditHero");
                        using (var response = await client.PutAsJsonAsync(url, sh))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            response.EnsureSuccessStatusCode();

                        }
                    }
                    return RedirectToAction("GetHeroes");
            }
        [HttpGet]
        public async Task<IActionResult> DeleteGame(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host}/api/VideoGames/Delete/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();


                    return RedirectToAction("GetGames");
                }
               

            }
        }



        public async Task<IActionResult> DeleteHero(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host}/api/SuperHero/Delete/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();



                    return RedirectToAction("GetHeroes");
                }
                return RedirectToAction("GetHeroes");

            }
        }

        public async Task<IActionResult> ViewGames()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var gamelist = new List<VideoGames>();

            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/VideoGames/GetGames");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    gamelist = JsonConvert.DeserializeObject<List<VideoGames>>(readtask);
                    return View(gamelist);
                }

            }
            return View(gamelist);
        }

        public async Task<IActionResult> ViewHeroes()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            var superlist = new List<Superheroes>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = ($"{host}/api/SuperHero/GetHeroes");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    superlist = JsonConvert.DeserializeObject<List<Superheroes>>(readtask);
                    return View(superlist);
                }
            }
            return View(superlist);
        }
        public async Task<IActionResult> AddGame()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGame(VideoGameEdit vg)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            Random rnd = new Random();
            string randNo = rnd.Next(10000, 9999999).ToString();
            var image = randNo + Path.GetExtension(vg.ImageFile.FileName);
            var path = _hostenvironment.WebRootPath;
            var filePath = "\\lib\\Images\\" + image;
            var fullPath = path+filePath;

            UploadFile(vg.ImageFile, fullPath);
            await AddGameImg(vg.Name, vg.Genre, vg.Developer, vg.Description, vg.size, vg.Rating, vg.Released, image);
            return RedirectToAction("GetGames");

        }
        public async Task<IActionResult> AddGameImg(string name, string genre, string developer, string description, int size, float rating, int released, string image)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            var vg = new VideoGames
            {
                Name=name,
                Genre=genre,
                Developer=developer,
                Description= description,
                size=size,
                Rating=rating,
                Image=image,
                Released=released,
                Category= "Game      "
            };

            using (var client = new HttpClient())
            {
                string url = $"{host}/api/VideoGames/AddGame";
                var response = await client.PostAsJsonAsync(url, vg);

                return RedirectToAction("GetGames");
            }

            return RedirectToAction("GetGames");
        }

        public async Task<IActionResult> AddHero()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> AddHero(SuperheroesEdit sh)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            Random rnd = new Random();
            string randNo = rnd.Next(1000000, 9999999).ToString();
            string image = randNo + Path.GetExtension(sh.ImageFile.FileName);
            string path = _hostenvironment.WebRootPath;
            string filePath = "\\lib\\Images\\Supe\\" + image;
            string fullPath = path + filePath;

            UploadFile(sh.ImageFile, fullPath);

           await AddheroImg( sh.Name, sh.FirstName, sh.LastName, sh.Powers, sh.Description, image);
            return RedirectToAction("GetHeroes");
        }
        public async Task<IActionResult> AddheroImg(string name, string firstname, string lastname, string powers, string description, string image)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }

            var vg = new Superheroes
            {
                
                Name = name,
                FirstName = firstname,
                LastName = lastname,
                Powers = powers,
                Description = description,
                Image = image
            };

            using (var client = new HttpClient())
            {
                string url = $"{host}/api/SuperHero/AddHero";
                var response = await client.PostAsJsonAsync(url, vg);

                return RedirectToAction("GetHeroes");
            }

            return RedirectToAction("Getheroes");
        }


        public async Task<IActionResult> GetMovies()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var gamelist = new List<Movieget>();

            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host2}/api/Movies/GetMovies");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    gamelist = JsonConvert.DeserializeObject<List<Movieget>>(readtask);
                    return View(gamelist);
                }

            }
            return View(gamelist);
        }
        public async Task<IActionResult> AddMovieImg()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieImg(ApiMovie mv)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");

            if (accesstoken == "Not")
            { return Redirect("~/Home"); }



            Random rnd = new Random();
            string randNo = rnd.Next(10000000, 99999999).ToString();

            string img = randNo + Path.GetExtension(mv.Image.FileName);

            var path = _hostenvironment.WebRootPath;
            var filepath = "\\lib\\Images\\" + img;
            var fullPath = path + filepath;

            

            UploadFile(mv.Image, fullPath);

            await AddMovie(mv.Name, mv.Director, mv.Released, img , mv.Rating, mv.Description);

            return RedirectToAction("GetMovies");
        }

        public async Task<ActionResult> AddMovie(string name, string director, int released, string img, float rating, string description)
        {
            string url = "https://localhost:44388/api/Movies/AddMovie";

            var test = new Movies
            {
                Name = name,
                Released = released,
                Director = director,
                Image = img.ToString(),
                Rating = rating,
                Description = description
            };

            string jsonData = JsonConvert.SerializeObject(test, Formatting.Indented);


            using (var cl = new HttpClient())
            {
                var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var res = await cl.PostAsync(url, httpContent);

                return RedirectToAction("GetMovies");

            }

        }

        public void UploadFile(IFormFile file, string filePath)
        {
            //Random rnd = new Random();
            //file.FileName = rnd.Next(1000, 9999).ToString() + Path.GetExtension(file.FileName);
            //FileStream stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(new FileStream(filePath, FileMode.Create));
            var p = Path.GetExtension(filePath);
        }

        public async Task<ActionResult> GetMoviesUser()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var gamelist = new List<Movieget>();

            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host2}/api/Movies/GetMovies");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    gamelist = JsonConvert.DeserializeObject<List<Movieget>>(readtask);
                    return View(gamelist);
                }

            }
            return View(gamelist);
        }
      
        public async Task<IActionResult> EditMovies(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                int movieid = id;

                string url = ($"{host2}/api/Movies/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<MovieEdit>(apiResponse);
                    return View(result);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMovies(MovieEdit mv)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }


            using (var client = new HttpClient())
            {
                if (mv.Imagefile == null)
                {
                    await EditMoviesImg(mv.Name, mv.Director, mv.Released, mv.Image, mv.Id, mv.Description, mv.Rating);
                }
                else
                {
                    Random rnd = new Random();
                    string randNo = rnd.Next(10000000, 99999999).ToString();

                    string img = randNo + mv.Imagefile.FileName;

                    var path = _hostenvironment.WebRootPath;
                    var file = "\\lib\\Images\\" + img;
                    var full = path + file;
                    string ext = Path.GetExtension(full);
                    string image = randNo + ext;
                    var filepath = "\\lib\\Images\\" + image;
                    var fullPath = path + filepath;




                    UploadFile(mv.Imagefile, fullPath);

                    await EditMoviesImg(mv.Name, mv.Director, mv.Released, image, mv.Id, mv.Description, mv.Rating);

                }

                return RedirectToAction("GetMovies");
            }

        }
        public async Task<ActionResult> EditMoviesImg(string name, string director,int released, string img, int id, string description, float rating)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            {
                return Redirect("~/Home");
            }
            var test = new Movieget
            {
                Name = name,   
                Director=director,
                Released=released,
                Image=img,
                Id=id,
                Description=description,
                Rating=rating



            };
            using (var client = new HttpClient())
                {


                    string url = ($"{host2}/api/Movies/EditMovie");
                using (var response = await client.PutAsJsonAsync(url, test))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                }
            }
                return RedirectToAction("GetMovies");
           
        }
        public async Task<IActionResult> DeleteMovie(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host2}/api/Movies/Delete/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();



                    return RedirectToAction("GetMovies");
                }
                return RedirectToAction("GetMovies");

            }


        }
        public async Task<IActionResult> BuyMovie(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                int movieid = id;

                string url = ($"{host2}/api/Movies/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Movieget>(apiResponse);
                    return View(result);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> OrderMovie(int id)
        {
            using (var client = new HttpClient())
            {
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                int movieid = id;

                string url = ($"{host2}/api/Movies/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Movieget>(apiResponse);

                    Cartmovie(result);
                    return RedirectToAction("AddedToCart");
                }
            }
        }
        [HttpPost]
        public async void Cartmovie(Movieget mv)
        {
            using (var client = new HttpClient())
            {
                string url2 = $"{host3}/api/Auth/show-Id";
                var response = await client.GetAsync(url2);
                string apiresponse = await response.Content.ReadAsStringAsync();
                var c = Convert.ToInt32(apiresponse);
            
            var cart = new Cart
            {
                productId = mv.Id,
                productName = mv.Name,
                productImage = mv.Image,
                productType = "Movie",
                productPrice= 60,
                UserId = c
                
                };

            


                string url = $"{host2}/api/Cart/AddProduct";
            
                var response2 = await client.PostAsJsonAsync(url, cart);
                
                    string apiResponse2 = await response2.Content.ReadAsStringAsync();
                   
                   
                
                
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddedToCart()
        {
            return View();
        }

        public async Task<ActionResult> ViewCart( int uid)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var cartlist = new List<Cart>();

            using (var client = new HttpClient())
            {
                string url2 = $"{host3}/api/Auth/show-Id";
                var response2 = await client.GetAsync(url2);
                string apiresponse2 = await response2.Content.ReadAsStringAsync();
                var c = Convert.ToInt32(apiresponse2);

                uid = c;
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host2}/api/Cart/GetAllProducts/{uid}");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    cartlist = JsonConvert.DeserializeObject<List<Cart>>(readtask);
                    return View(cartlist);
                }

            }
            return View(cartlist);


        }
        public async Task<IActionResult> GetGamesUser()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            var gamelist = new List<VideoGames>();

            using (var client = new HttpClient())
            {
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/VideoGames/GetGames");
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var readtask = response.Content.ReadAsStringAsync().Result;
                    gamelist = JsonConvert.DeserializeObject<List<VideoGames>>(readtask);
                    return View(gamelist);
                }

            }
            return View(gamelist);
        }
        public async Task<IActionResult> BuyGames(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);


                string url = ($"{host}/api/VideoGames/GetGames/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<VideoGames>(apiResponse);
                    return View(result);
                }


            }
        }

        public async Task<IActionResult> OrderGames(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host}/api/VideoGames/GetGames/{id}");
                using (var response = await client.GetAsync(url))
                {
                    
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<VideoGames>(apiresponse);
                    Cartgame(result);
                    return RedirectToAction("AddedToCart");
                }
            }
        }
        [HttpPost]
        public async void Cartgame(VideoGames mv)
        {
            using (var client = new HttpClient())
            {
                string url2 = $"{host3}/api/Auth/show-Id";
                var response2 = await client.GetAsync(url2);
                string apiresponse2 = await response2.Content.ReadAsStringAsync();
                var c = Convert.ToInt32(apiresponse2);
                var cart = new Cart
                {
                    productId = mv.Id,
                    productName = mv.Name,
                    productImage = mv.Image,
                    productType = "Game",
                    productPrice = 70,
                    UserId = c

                };




                string url = $"{host2}/api/Cart/AddProduct";
           
                var response = await client.PostAsJsonAsync(url, cart);

                string apiResponse = await response.Content.ReadAsStringAsync();




            }
        }
        public async Task<IActionResult> HeroDetails(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);


                string url = ($"{host}/api/SuperHero/GetHeroes/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Superheroes>(apiResponse);
                    return View(result);
                }


            }
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host2}/api/Cart/DeleteProduct/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();



                    return RedirectToAction("ViewCart");
                }
                return RedirectToAction("ViewCart");

            }


        }
        public async Task<IActionResult> ConfirmOrder(Cart cart)
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not") { return Redirect("~/Home"); }
            


            using (var client = new HttpClient())
            {
                var a = await client.GetAsync(idget);
                var b = await a.Content.ReadAsStringAsync();
                var uid = Convert.ToInt32(b);
                string url2 = ($"{host3}/api/Auth/show-Id");
                string url3 = ($"{host2}/api/Cart/CartCount/{uid}");
                string url4 = ($"{host2}/api/Cart/TotalPrice/{uid}");
                var totalp = await client.GetAsync(url4);
                var c = await totalp.Content.ReadAsStringAsync();
                var totali = await client.GetAsync(url3);
                var d = await totali.Content.ReadAsStringAsync();
             

                var ord = new Ord
                {
                    Time = DateTime.Now,
                    Orderedby = Convert.ToInt16(uid),
                    TotalItems = Convert.ToInt16(d),
                    TotalPrice = Convert.ToInt16(c)
                };
                string url = $"{host2}/api/Orders/AddOrder";



                var response = await client.PostAsJsonAsync(url, ord);

                string apiResponse = await response.Content.ReadAsStringAsync();



                ClearCart();



            }
            return RedirectToAction("ViewOrder");


        }
        public async Task<IActionResult> ViewOrder()
        {
            return View();
        }

        public async void ClearCart()
        {
        

            using (var client = new HttpClient())
            {
                string url2 = "https://localhost:44393/api/Auth/show-Id";
                var a = await client.GetAsync(url2);
                var b = await a.Content.ReadAsStringAsync();
                var uid = Convert.ToInt16(b);
                var host = "https://localhost:44388";
                string url = ($"{host}/api/Cart/DeleteCart/{uid}");

                var c = await client.DeleteAsync(url);

                var apiresponse = c.Content.ReadAsStringAsync();
       
            }
        }
        public async Task<IActionResult> DeleteCart()
        {


            using (var client = new HttpClient())
            {
                string url2 = "https://localhost:44393/api/Auth/show-Id";
                var a = await client.GetAsync(url2);
                var b = await a.Content.ReadAsStringAsync();
                var uid = Convert.ToInt16(b);
                var host = "https://localhost:44388";
                string url = ($"{host}/api/Cart/DeleteCart/{uid}");

                var c = await client.DeleteAsync(url);

                var apiresponse = c.Content.ReadAsStringAsync();

            }
            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> GetAllOrders()
        {
            accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not") { return Redirect("~/Home"); }

            string url = ($"{host}/api/Orders/GetAllOrders");

            using (var client = new HttpClient())
            {
                var c = await client.GetAsync(url);
                var response = c.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<List<Orders>>(response);
                return View(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);


                string url = ($"{host}/api/Orders/GetOrderbyId/{id}");
                using (var response = await client.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Orders>(apiResponse);
                    return View(result);
                }


            }
        }
        [HttpPost]
        public async Task<IActionResult> EditOrder(Orders ord)
        {
            var accesstoken = HttpContext.Session.GetString("JWTToken");
            if (accesstoken == "Not")
            { return Redirect("~/Home"); }
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);
                string url = ($"{host}/api/Orders/EditOrder");
                using (var response = await client.PutAsJsonAsync(url, ord))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                }
            }
            return RedirectToAction("GetAllOrders");
        }

        public async Task<IActionResult> DeleteOrder(int id)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accesstoken);

                string url = ($"{host}/api/Orders/DeleteOrder/{id}");
                using (var response = await client.DeleteAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();



                    return RedirectToAction("GetAllOrders");
                }
                return RedirectToAction("GetAllOrders");

            }


        }
        

       
        public async Task<ActionResult> Search(string keyword)
        {
            using (var client = new HttpClient())
            {
                var accesstoken = HttpContext.Session.GetString("JWTToken");
                if (accesstoken == "Not")
                { return Redirect("~/Home"); }

                string url = $"{host}/api/Cart/Search/{keyword}";
                using (var response = await client.GetAsync(url))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<Search>>(apiresponse);
                    return View(result);
                }
            }
        }
    }
}

