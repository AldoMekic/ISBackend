using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
    [Route("api/grades")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GradesController(IGradeService gradeService, IUserService userService, IMapper mapper)
        {
            _gradeService = gradeService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grades = await _gradeService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Grade?>, IEnumerable<GradeResponseDTO>>(grades));
        }

        [HttpGet("{id:int}", Name = nameof(GetGrade))]
        public async Task<IActionResult> GetGrade(int id)
        {
            var grade = await _gradeService.GetById(id);
            return Ok(_mapper.Map<Grade, GradeResponseDTO>(grade));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeRequestDTO grade)
        {
            var created = await _gradeService.CreateGrade(grade);
            return CreatedAtAction(nameof(GetGrade), new { id = created.Id }, _mapper.Map<Grade, GradeResponseDTO>(created));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _gradeService.GetById(id);
            await _gradeService.RemoveGrade(grade);
            return NoContent();
        }

        [HttpPost("{id:int}/request-annulment")]
        public async Task<IActionResult> RequestAnnulment(int id)
        {
            var updated = await _gradeService.RequestAnnulment(id);
            // reload fully for enriched mapping
            var full = await _gradeService.GetById(updated.Id);
            return Ok(_mapper.Map<GradeResponseDTO>(full));
        }

        [Authorize]
        [HttpPut("{id:int}/request-annulment")]
        public async Task<IActionResult> RequestAnnulmentPut(int id)
        {
            var userIdStr =
                User.FindFirstValue("user_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(new { message = "Missing user id claim." });

            var updated = await _gradeService.RequestAnnulment(id, userId);

            // reload fully for enriched mapping (keeps your current behavior)
            var full = await _gradeService.GetById(updated.Id);
            return Ok(_mapper.Map<GradeResponseDTO>(full));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyGrades()
        {
            // ✅ Prefer user_id / NameIdentifier as USER ID (because your JWT now includes it)
            var userIdStr =
                User.FindFirstValue("user_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdStr, out var userId) && userId > 0)
            {
                // Optional: ensure user exists and is student
                var user = await _userService.GetById(userId);
                if (user == null) return Unauthorized("Can't find user");
                if (!user.IsStudent) return Forbid();

                var grades = await _gradeService.GetGradesForStudentUserId(userId);
                return Ok(grades);
            }

            // Fallback: old behavior (username-based)
            var username =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized("No username");

            var u2 = await _userService.GetByUsername(username);
            if (u2 == null) return Unauthorized("Can't find user");
            if (!u2.IsStudent) return Forbid();

            var grades2 = await _gradeService.GetGradesForStudentUserId(u2.Id);
            return Ok(grades2);
        }

        [Authorize]
        [HttpGet("my")]
        public Task<IActionResult> GetMyGradesAlias()
        {
            return GetMyGrades(); // re-use existing implementation (/me)
        }
    }
}
