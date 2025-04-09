using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskAssignmentsController : ControllerBase
    {
        private readonly ITaskAssignmentRepository _repository;
        private readonly IMapper _mapper;

        public TaskAssignmentsController(ITaskAssignmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TaskAssignmentDto>>(data));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var data = await _repository.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<TaskAssignmentDto>>(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskAssignmentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var assignment = _mapper.Map<TaskAssignmentModel>(request);
            var created = await _repository.CreateAsync(assignment);
            return Ok(_mapper.Map<TaskAssignmentDto>(created));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted ? Ok("Assignment deleted.") : NotFound();
        }

        [HttpGet("user/{userId}/status/{status}")]
        public async Task<IActionResult> GetTasksByUserAndStatus(int userId, string status)
        {
            var tasks = await _repository.GetByUserIdAsync(userId);
            var filtered = tasks.Where(t => t.Task.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            return Ok(_mapper.Map<IEnumerable<TaskAssignmentDto>>(filtered));
        }
    }
}
