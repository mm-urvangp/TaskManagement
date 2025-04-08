using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Models.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories.IRepositories;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public UsersController(IUserRepository repo, IMapper mapper, IWebHostEnvironment env)
        {
            _repo = repo;
            _mapper = mapper;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _repo.GetByEmailAsync(request.Email) is not null)
                return BadRequest("Email already exists.");

            if (await _repo.GetByMobileAsync(request.Mobile) is not null)
                return BadRequest("Mobile already exists.");

            if (request.ProfilePic != null && request.ProfilePic.Length > 3 * 1024 * 1024)
                return BadRequest("Profile pic must be under 3MB.");

            string? profilePath = null;

            if (request.ProfilePic != null)
            {
                string uploads = Path.Combine(_env.EnvironmentName, "profiles");
                Directory.CreateDirectory(uploads);

                string fileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePic.FileName);
                profilePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(profilePath, FileMode.Create);
                await request.ProfilePic.CopyToAsync(stream);
            }

            var user = _mapper.Map<UserModel>(request);
            user.ProfilePicPath = profilePath;

            var created = await _repo.CreateAsync(user);
            return Ok(_mapper.Map<UserDto>(created));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user is not null
                ? Ok(_mapper.Map<UserDto>(user))
                : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UserCreateRequest request)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                && await _repo.GetByEmailAsync(request.Email) is not null)
                return BadRequest("Email already exists.");

            if (user.Mobile != request.Mobile && await _repo.GetByMobileAsync(request.Mobile) is not null)
                return BadRequest("Mobile already exists.");

            if (request.ProfilePic != null && request.ProfilePic.Length > 3 * 1024 * 1024)
                return BadRequest("Profile pic must be under 3MB.");

            if (request.ProfilePic != null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicPath) && System.IO.File.Exists(user.ProfilePicPath))
                    System.IO.File.Delete(user.ProfilePicPath);

                string uploads = Path.Combine(_env.WebRootPath, "profiles");
                Directory.CreateDirectory(uploads);

                string fileName = Guid.NewGuid() + Path.GetExtension(request.ProfilePic.FileName);
                string filePath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ProfilePic.CopyToAsync(stream);

                user.ProfilePicPath = filePath;
            }

            _mapper.Map(request, user);
            bool updated = await _repo.UpdateAsync(user);
            return updated ? Ok(_mapper.Map<UserDto>(user)) : StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.ProfilePicPath) && System.IO.File.Exists(user.ProfilePicPath))
                System.IO.File.Delete(user.ProfilePicPath);

            bool deleted = await _repo.DeleteAsync(id);
            return deleted ? Ok("User deleted.") : StatusCode(500);
        }
    }
}
