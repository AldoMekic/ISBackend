using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentApplication.Business.Services;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StudentApplication.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _enrollmentService.GetAll();
            return Ok(_mapper.Map<IEnumerable<Enrollment?>, IEnumerable<EnrollmentResponseDTO>>(all));
        }

        [HttpGet("{id:int}", Name = nameof(GetEnrollment))]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);
            return Ok(_mapper.Map<Enrollment, EnrollmentResponseDTO>(enrollment));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequestDTO enrollment)
        {
            var created = await _enrollmentService.CreateEnrollment(enrollment);
            var dto = _mapper.Map<Enrollment, EnrollmentResponseDTO>(await _enrollmentService.GetById(created.Id));
            return CreatedAtAction(nameof(GetEnrollment), new { id = created.Id }, dto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetById(id);
            await _enrollmentService.RemoveEnrollment(enrollment);
            return NoContent();
        }

        [HttpPut("{id:int}/drop")]
        public async Task<IActionResult> Drop(int id)
        {
            var e = await _enrollmentService.Drop(id);
            var dto = _mapper.Map<EnrollmentResponseDTO>(await _enrollmentService.GetById(e.Id));
            return Ok(dto);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var e = await _enrollmentService.Complete(id);
            var dto = _mapper.Map<EnrollmentResponseDTO>(await _enrollmentService.GetById(e.Id));
            return Ok(dto);
        }

        public class SetStatusRequest { public EnrollmentStatus Status { get; set; } }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromBody] SetStatusRequest req)
        {
            var e = await _enrollmentService.SetStatus(id, req.Status);
            var dto = _mapper.Map<EnrollmentResponseDTO>(await _enrollmentService.GetById(e.Id));
            return Ok(dto);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userIdStr =
                User.FindFirstValue("user_id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
                return Unauthorized(new { message = "Missing user id claim." });

            var mine = await _enrollmentService.GetForStudentUserId(userId);
            return Ok(_mapper.Map<IEnumerable<Enrollment>, IEnumerable<EnrollmentResponseDTO>>(mine));
        }
    }
}
