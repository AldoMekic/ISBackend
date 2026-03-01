using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StudentApplication.Controllers
{
    //[Authorize(Policy ="AdminOnly")]
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IUserService userService, IMapper mapper)
        {
            _adminService = adminService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Admin?>, IEnumerable<AdminResponseDTO>>(admins));
        }

        [HttpGet("{id:int}", Name = nameof(GetAdmin))]
        public async Task<IActionResult> GetAdmin(int id)
        {
            var admin = await _adminService.GetById(id);
            return Ok(_mapper.Map<Admin, AdminResponseDTO>(admin));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRequestDTO admin)
        {
            await _adminService.CreateAdmin(admin);
            var created = (await _adminService.GetAll()).First(a => a!.Username == admin.Username)!;
            return CreatedAtAction(nameof(GetAdmin), new { id = created.Id }, _mapper.Map<Admin, AdminResponseDTO>(created));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetById(id);
            await _adminService.RemoveAdmin(admin);
            return NoContent();
        }

        // ====== EXISTING (kept): returns Users only ======
        [HttpGet("unapproved-professors")]
        public async Task<IActionResult> GetUnapprovedProfessors()
        {
            var pending = await _userService.GetUnapprovedProfessors();
            var result = pending.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email
            });
            return Ok(result);
        }

        // ====== ✅ NEW: returns Professor profiles with User + Department (matches frontend approvals table) ======
        [HttpGet("pending-professor-profiles")]
        public async Task<IActionResult> GetPendingProfessorProfiles()
        {
            var pending = await _userService.GetPendingProfessorProfiles();

            // shape matches your frontend usage: professor.user.first_name, professor.department.name, professor.title, professor.user.age
            var result = pending.Select(p => new
            {
                id = p.Id,
                title = p.Title,
                isApproved = p.IsApproved,
                approvedAt = p.ApprovedAt,
                department = p.Department == null ? null : new
                {
                    id = p.Department.Id,
                    name = p.Department.Name,
                },
                user = p.User == null ? null : new
                {
                    id = p.User.Id,
                    username = p.User.Username,
                    email = p.User.Email,
                    first_name = p.User.Username, // fallback if User has no FirstName field in your model
                    last_name = "",              // fallback
                    age = p.Age                  // use Professor.Age because User likely has no Age
                }
            });

            return Ok(result);
        }

        [HttpPut("approve/{userId:int}")]
        public async Task<IActionResult> ApproveProfessor(int userId)
        {
            // ✅ Our JWT includes "user_id" and NameIdentifier. Prefer user_id.
            var adminUserIdStr =
                User?.FindFirst("user_id")?.Value ??
                User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(adminUserIdStr) || !int.TryParse(adminUserIdStr, out var adminUserId))
                return Unauthorized(new { message = "Missing admin user id claim." });

            // ✅ Prefer JWT sub (username). Identity.Name is often null unless you map it explicitly.
            var adminName =
                User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User?.Identity?.Name ??
                "admin";

            await _userService.ApproveProfessor(userId, adminUserId, adminName);
            return NoContent();
        }
    }
}