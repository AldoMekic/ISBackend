using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace StudentApplication.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IUserService userService, IMapper mapper)
        {
            _studentService = studentService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _studentService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Student?>, IEnumerable<StudentResponseDTO>>(all));
        }

        [HttpGet("{id:int}", Name = nameof(GetStudent))]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _studentService.GetById(id);
            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetStudentByName(string name)
        {
            var student = await _studentService.GetByName(name);
            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRequestDTO student)
        {
            await _studentService.CreateStudent(student);
            var created = await _studentService.GetFirst(); // simple way; replace with freshly created fetch if needed
            return CreatedAtAction(nameof(GetStudent), new { id = created.Id }, _mapper.Map<Student, StudentResponseDTO>(created));
        }

        // === Subject flows via Enrollments (mirrors your previous commented design) ===

        [HttpPost("{studentId:int}/subjects/{subjectId:int}")]
        public async Task<IActionResult> AddSubjectToStudent(int studentId, int subjectId)
        {
            await _studentService.EnrollStudentInSubject(studentId, subjectId);
            var student = await _studentService.GetById(studentId);
            return Ok(_mapper.Map<Student, StudentResponseDTO>(student));
        }

        [HttpGet("{studentId:int}/subjects")]
        public async Task<IActionResult> GetStudentSubjects(int studentId)
        {
            var subjects = await _studentService.GetStudentSubjects(studentId);
            var dto = _mapper.Map<List<SubjectResponseDTO>>(subjects);
            return Ok(dto);
        }

        [HttpDelete("{studentId:int}/subjects/{subjectId:int}")]
        public async Task<IActionResult> RemoveStudentSubject(int studentId, int subjectId)
        {
            await _studentService.UnenrollStudentFromSubject(studentId, subjectId);
            return NoContent();
        }

        // === Grades listing/removal for a student ===

        [HttpGet("{studentId:int}/grades")]
        public async Task<IActionResult> GetAllGrades(int studentId)
        {
            var grades = await _studentService.GetStudentGrades(studentId);
            var dto = _mapper.Map<List<GradeResponseDTO>>(grades);
            return Ok(dto);
        }

        [HttpDelete("{studentId:int}/grades/{gradeId:int}")]
        public async Task<IActionResult> RemoveStudentGrade(int studentId, int gradeId)
        {
            await _studentService.RemoveStudentGrade(studentId, gradeId);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentService.GetById(id);
            await _studentService.RemoveStudent(student);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            // Your JWT includes these:
            // - "user_id" claim (string int)
            // - ClaimTypes.NameIdentifier (string int)
            var userIdRaw =
                User.FindFirstValue("user_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdRaw) || !int.TryParse(userIdRaw, out var userId))
                return Unauthorized(new { message = "Missing or invalid user_id claim." });

            var student = await _studentService.GetByUserId(userId);
            return Ok(_mapper.Map<StudentResponseDTO>(student));
        }
    }
}
