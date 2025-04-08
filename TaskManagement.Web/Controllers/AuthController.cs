using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskManagement.Web.Models;
using TaskManagement.Web.Services;

namespace TaskManagement.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _config;

        public AuthController(ApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var apiUrl = _config["ApiBaseUrl"] + "/Auth/login";
            var response = await _apiService.PostAsync(apiUrl, model);

            if (response.IsSuccessStatusCode)
            {
                var loginData = JsonConvert.DeserializeObject<LoginResponse>(
                    await response.Content.ReadAsStringAsync());

                HttpContext.Session.SetString("JWToken", loginData.Token);
                HttpContext.Session.SetInt32("UserId", loginData.UserId);
                HttpContext.Session.SetString("Role", loginData.Role);

                //return RedirectToAction("Dashboard", "Task");

                return loginData.Role == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("MyTasks", "User");
            }

            ViewBag.Error = "Invalid login";
            return View();
        }
    }
}
