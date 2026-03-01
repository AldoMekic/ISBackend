using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
    [Route("api/professors")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly IMapper _mapper;

        public ProfessorsController(IProfessorService professorService, IMapper mapper)
        {
            _professorService = professorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profs = await _professorService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Professor?>, IEnumerable<ProfessorResponseDTO>>(profs));
        }

        [HttpGet("{id:int}", Name = nameof(GetProfessor))]
        public async Task<IActionResult> GetProfessor(int id)
        {
            var professor = await _professorService.GetById(id);
            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetProfessorByName(string name)
        {
            var professor = await _professorService.GetByName(name);
            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfessor([FromBody] ProfessorRequestDTO professor)
        {
            await _professorService.CreateProfessor(professor);
            var created = await _professorService.GetFirst(); // simplest way to fetch; replace with GetByName if needed
            return CreatedAtAction(nameof(GetProfessor), new { id = created.Id }, _mapper.Map<Professor, ProfessorResponseDTO>(created));
        }

        // Assign or reassign subject to professor
        [HttpPut("{professorId:int}/subjects/{subjectId:int}")]
        public async Task<IActionResult> AssignSubjectToProfessor(int professorId, int subjectId)
        {
            await _professorService.AddSubjectToProfessor(professorId, subjectId);
            var professor = await _professorService.GetById(professorId);
            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpPut("reassign-subject/{subjectId:int}/to/{newProfessorId:int}")]
        public async Task<IActionResult> ReassignProfessorSubject(int subjectId, int newProfessorId)
        {
            await _professorService.ReassignProfessorSubject(subjectId, newProfessorId);
            var professor = await _professorService.GetById(newProfessorId);
            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }

        [HttpGet("{professorId:int}/subjects")]
        public async Task<IActionResult> GetAllSubjects(int professorId)
        {
            var subjects = await _professorService.GetProfessorSubjects(professorId);
            var dto = _mapper.Map<List<SubjectResponseDTO>>(subjects);
            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProfessor(int id)
        {
            var professor = await _professorService.GetById(id);
            await _professorService.RemoveProfessor(professor);
            return NoContent();
        }

        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            var profs = await _professorService.GetApproved();
            return Ok(_mapper.Map<IEnumerable<ProfessorResponseDTO>>(profs));
        }

        // ✅ NEW: logged-in professor profile
        // This is what allows frontend to stop using localStorage professor_id
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // Try common claim types
            var uidStr =
                User.FindFirstValue("user_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(uidStr, out var userId))
                return Unauthorized(new { message = "Missing user id claim." });

            var professor = await _professorService.GetByUserId(userId);
            return Ok(_mapper.Map<Professor, ProfessorResponseDTO>(professor));
        }
    }
}