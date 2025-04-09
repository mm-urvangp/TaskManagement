using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using TaskManagement.Web.Models.DTOs;

namespace TaskManagement.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AdminController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        #region Dashboard

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                ViewBag.Role = "Admin";
            }
            else
            {
                ViewBag.Role = "User";
            }
            return View();
        }

        #endregion

        #region TASKS 

        public async Task<IActionResult> Tasks()
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Tasks");
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            return View(tasks);
        }

        public IActionResult CreateTask() => View();

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskDto model)
        {
            var client = CreateClient();

            if (model.UploadFile != null && model.UploadFile.Length > 2 * 1024 * 1024)
                return BadRequest("File size must be less than 2MB.");

            using var content = new MultipartFormDataContent();

            // Add string fields
            content.Add(new StringContent(model.TaskTitle ?? ""), "TaskTitle");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.Status ?? ""), "Status");
            content.Add(new StringContent(model.Priority ?? ""), "Priority");
            if (model.DueDate != null)
                content.Add(new StringContent(model.DueDate.ToString("yyyy-MM-dd")), "DueDate");

            // Add file if available
            if (model.UploadFile != null && model.UploadFile.Length > 0)
            {
                var fileContent = new StreamContent(model.UploadFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.UploadFile.ContentType);
                content.Add(fileContent, "UploadFile", model.UploadFile.FileName);
            }

            var response = await client.PostAsync($"{_config["ApiBaseUrl"]}/Tasks", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Tasks");

            ModelState.AddModelError("", "Unable to create task");
            return View(model);
        }

        public async Task<IActionResult> EditTask(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Tasks/{id}");
            var task = await response.Content.ReadFromJsonAsync<TaskDto>();
            return View("CreateTask", task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(TaskDto model)
        {
            var client = CreateClient();

            if (model.UploadFile != null && model.UploadFile.Length > 2 * 1024 * 1024)
                return BadRequest("File size must be less than 2MB.");

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Id.ToString()), "Id");
            content.Add(new StringContent(model.TaskTitle ?? ""), "TaskTitle");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.Status ?? "Pending"), "Status");
            content.Add(new StringContent(model.Priority ?? "Low"), "Priority");
            content.Add(new StringContent(model.DueDate.ToString("yyyy-MM-dd") ?? ""), "DueDate");

            if (model.UploadFile != null && model.UploadFile.Length > 0)
            {
                var streamContent = new StreamContent(model.UploadFile.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.UploadFile.ContentType);
                content.Add(streamContent, "UploadFile", model.UploadFile.FileName);
            }

            var response = await client.PutAsync($"{_config["ApiBaseUrl"]}/Tasks/{model.Id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Tasks");

            ModelState.AddModelError("", "Unable to update task");
            return View(model);
        }

        public async Task<IActionResult> DeleteTask(int id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"{_config["ApiBaseUrl"]}/Tasks/{id}");
            return RedirectToAction("Tasks");
        }
        #endregion

        #region USERS 

        public async Task<IActionResult> Users()
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Users");
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto model)
        {
            var client = CreateClient();

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name ?? ""), "Name");
            content.Add(new StringContent(model.Email ?? ""), "Email");
            content.Add(new StringContent(model.Password ?? ""), "Password");
            content.Add(new StringContent(model.Gender ?? ""), "Gender");
            content.Add(new StringContent(model.Mobile ?? ""), "Mobile");
            content.Add(new StringContent(model.Role ?? ""), "Role");
            if (model.DateOfBirth != null)
                content.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");

            if (model.ProfilePic != null && model.ProfilePic.Length > 0)
            {
                var fileContent = new StreamContent(model.ProfilePic.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ProfilePic.ContentType);
                content.Add(fileContent, "ProfilePic", model.ProfilePic.FileName);
            }

            var response = await client.PostAsync($"{_config["ApiBaseUrl"]}/Users", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Users");

            ModelState.AddModelError(string.Empty, "Something went wrong!");
            return View(model);
        }

        // GET: EditUser
        public async Task<IActionResult> EditUser(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/Users/{id}");
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user != null)
                ViewBag.Age = DateTime.Now.Year - user.DateOfBirth.Year;
            return View("CreateUser", user);
        }

        // POST: EditUser
        [HttpPost]
        public async Task<IActionResult> EditUser(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = CreateClient();

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Id.ToString()), "Id");
            content.Add(new StringContent(model.Name ?? ""), "Name");
            content.Add(new StringContent(model.Email ?? ""), "Email");
            content.Add(new StringContent(model.Password ?? ""), "Password");
            content.Add(new StringContent(model.Gender ?? ""), "Gender");
            content.Add(new StringContent(model.Mobile ?? ""), "Mobile");
            content.Add(new StringContent(model.Role ?? ""), "Role");
            if (model.DateOfBirth != null)
                content.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), "DateOfBirth");

            if (model.ProfilePic != null && model.ProfilePic.Length > 0)
            {
                var fileContent = new StreamContent(model.ProfilePic.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ProfilePic.ContentType);
                content.Add(fileContent, "ProfilePic", model.ProfilePic.FileName);
            }

            var response = await client.PutAsync($"{_config["ApiBaseUrl"]}/Users/{model.Id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Users");

            ModelState.AddModelError(string.Empty, "Update failed.");
            return View("CreateUser", model);
        }

        // POST: DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"{_config["ApiBaseUrl"]}/Users/{id}");

            return RedirectToAction("Users");
        }

        #endregion

        #region ASSIGN TASK 

        public async Task<IActionResult> AssignedTasksList()
        {
            var client = CreateClient();
            var response = await client.GetAsync($"{_config["ApiBaseUrl"]}/TaskAssignments");
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskAssignmentDto>>();
            return View(tasks);
        }

        public async Task<IActionResult> AssignTask()
        {
            var client = CreateClient();

            var users = await client.GetFromJsonAsync<List<UserDto>>($"{_config["ApiBaseUrl"]}/Users");
            var tasks = await client.GetFromJsonAsync<List<TaskDto>>($"{_config["ApiBaseUrl"]}/Tasks");

            ViewBag.Users = users;
            ViewBag.Tasks = tasks;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignTask(int taskId, int userId)
        {
            var assignment = new
            {
                TaskId = taskId,
                UserId = userId,
                AssignedDate = DateTime.UtcNow
            };

            var client = CreateClient();

            var content = new StringContent(
                JsonConvert.SerializeObject(assignment),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"{_config["ApiBaseUrl"]}/TaskAssignments", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Task assigned successfully.";
                return RedirectToAction("Dashboard");
            }

            // Reload users and tasks in case of failure (needed for ViewBag)
            var usersResponse = await client.GetFromJsonAsync<List<UserDto>>($"{_config["ApiBaseUrl"]}/Users");
            var tasksResponse = await client.GetFromJsonAsync<List<TaskDto>>($"{_config["ApiBaseUrl"]}/Tasks");

            ViewBag.Users = usersResponse;
            ViewBag.Tasks = tasksResponse;

            ModelState.AddModelError("", "Failed to assign task");
            return View();
        }

        #endregion
    }
}
