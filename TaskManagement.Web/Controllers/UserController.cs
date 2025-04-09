using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using TaskManagement.Web.Models;
using TaskManagement.Web.Services;

namespace TaskManagement.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _config;

        public UserController(ApiService apiService, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _apiService = apiService;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> MyTasks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var apiUrl = $"{_config["ApiBaseUrl"]}/TaskAssignments/user/{userId}";

            var tasks = await _apiService.GetAsync<List<TaskViewModel>>(apiUrl);

            var pending = tasks.Where(t => t.Status == "Pending").ToList();
            var completed = tasks.Where(t => t.Status == "Completed").ToList();

            ViewBag.PendingTasks = pending;
            ViewBag.CompletedTasks = completed;

            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View(tasks);
        }

        public async Task<IActionResult> UpdateStatus(int taskId, string newStatus)
        {
            var client = CreateClient();

            // Serialize string directly as JSON content (e.g., "completed")
            var json = JsonConvert.SerializeObject(newStatus); // becomes a JSON string like: "completed"
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_config["ApiBaseUrl"]}/Tasks/UpdateStatus/{taskId}", content);

            return RedirectToAction("MyTasks");
        }
    }
}
