using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models;
using TaskManagement.API.Models.DTOs;
using TaskManagement.API.Repositories.IRepositories;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public TasksController(ITaskRepository taskRepo, IWebHostEnvironment env, IMapper mapper)
        {
            _taskRepo = taskRepo;
            _env = env;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _taskRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _taskRepo.GetByIdAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TaskCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.UploadFile != null && request.UploadFile.Length > 2 * 1024 * 1024)
                return BadRequest("File size must be less than 2MB.");

            string? filePath = null;
            if (request.UploadFile != null)
            {
                string uploads = Path.Combine(_env.EnvironmentName, "uploads");
                Directory.CreateDirectory(uploads);

                string fileName = Guid.NewGuid() + Path.GetExtension(request.UploadFile.FileName);
                filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.UploadFile.CopyToAsync(stream);
                }
            }

            var task = _mapper.Map<TaskModel>(request);
            task.UploadFilePath = filePath;

            var createdTask = await _taskRepo.AddAsync(task);

            return Ok(_mapper.Map<TaskDto>(createdTask));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] TaskCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = await _taskRepo.GetByIdAsync(id);
            if (existingTask == null)
                return NotFound("Task not found.");

            if (request.UploadFile != null && request.UploadFile.Length > 2 * 1024 * 1024)
                return BadRequest("File size must be less than 2MB.");

            if (request.UploadFile != null)
            {
                string uploads = Path.Combine(_env.EnvironmentName, "uploads");
                Directory.CreateDirectory(uploads);

                if (!string.IsNullOrEmpty(existingTask.UploadFilePath) && System.IO.File.Exists(existingTask.UploadFilePath))
                    System.IO.File.Delete(existingTask.UploadFilePath);

                string fileName = Guid.NewGuid() + Path.GetExtension(request.UploadFile.FileName);
                string filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.UploadFile.CopyToAsync(stream);
                }

                existingTask.UploadFilePath = filePath;
            }

            _mapper.Map(request, existingTask); // Updates properties from request except UploadFilePath

            bool updated = await _taskRepo.UpdateAsync(existingTask);
            return updated ? Ok(_mapper.Map<TaskDto>(existingTask)) : StatusCode(500, "Update failed.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskRepo.GetByIdAsync(id);
            if (task == null)
                return NotFound("Task not found.");

            // Delete associated file if exists
            if (!string.IsNullOrEmpty(task.UploadFilePath) && System.IO.File.Exists(task.UploadFilePath))
            {
                System.IO.File.Delete(task.UploadFilePath);
            }

            bool deleted = await _taskRepo.DeleteAsync(id);
            return deleted ? Ok("Task deleted.") : StatusCode(500, "Delete failed.");
        }

        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var task = await _taskRepo.GetByIdAsync(id);
            if (task == null)
                return NotFound("Task not found.");

            task.Status = status;
            bool changed = await _taskRepo.UpdateAsync(task);
            return changed ? Ok("Task Updated.") : StatusCode(500, "Update failed.");
        }
    }
}
