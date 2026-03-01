using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class GradeService : IGradeService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public GradeService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task<Grade> CreateGrade(GradeRequestDTO model)
        {
            // Load enrollment and prevent duplicates
            var enrollment = await _databaseContext.Enrollments
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(e => e.Id == model.EnrollmentId);

            if (enrollment == null) throw new KeyNotFoundException("Enrollment not found");
            if (enrollment.Grade != null) throw new InvalidOperationException("This enrollment already has a grade.");

            // Create grade
            var grade = _mapper.Map<Grade>(model);
            await _databaseContext.Grades.AddAsync(grade);
            await _databaseContext.SaveChangesAsync();

            // Auto-complete the enrollment
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTimeOffset.UtcNow;
            await _databaseContext.SaveChangesAsync();

            return grade;
        }

        public async Task<IEnumerable<Grade?>> GetAll()
        {
            return await _databaseContext.Grades
                .AsNoTracking()
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Subject)
                        .ThenInclude(s => s.Professor)
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Student)
                .ToListAsync();
        }

        public async Task<Grade> GetById(int id)
        {
            var result = await _databaseContext.Grades
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Subject)
                        .ThenInclude(s => s.Professor)
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (result == null) throw new KeyNotFoundException("Grade not found");
            return result;
        }

        public async Task RemoveGrade(Grade grade)
        {
            _databaseContext.Grades.Remove(grade);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<Grade> RequestAnnulment(int gradeId)
        {
            var g = await _databaseContext.Grades.FirstOrDefaultAsync(x => x.Id == gradeId)
                    ?? throw new KeyNotFoundException("Grade not found");

            var within3Days = (DateTimeOffset.UtcNow - g.AssignedAt).TotalDays <= 3;
            if (!within3Days)
                throw new InvalidOperationException("Annulment window (3 days) has expired.");

            if (g.AnnulmentRequested) return g;

            g.AnnulmentRequested = true;
            g.AnnulmentRequestedAt = DateTimeOffset.UtcNow;
            await _databaseContext.SaveChangesAsync();
            return g;
        }


        public async Task UpdateGrade(Grade grade)
        {
            _databaseContext.Update(grade);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<GradeResponseDTO>> GetGradesForStudentUserId(int userId)
        {
            var now = DateTimeOffset.UtcNow;

            var grades = await _databaseContext.Grades
                .AsNoTracking()
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Subject)
                        .ThenInclude(s => s.Professor)
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Student)
                .Where(g =>
                    g.Enrollment.Student.UserId == userId
                )
                .Select(g => new GradeResponseDTO
                {
                    Id = g.Id,
                    EnrollmentId = g.EnrollmentId,
                    OfficialGrade = g.OfficialGrade,
                    TotalScore = g.TotalScore,
                    AssignedAt = g.AssignedAt,

                    SubjectName = g.Enrollment.Subject.Title,
                    StudentName = g.Enrollment.Student.FirstName + " " +
                                  g.Enrollment.Student.LastName,
                    ProfessorName = g.Enrollment.Subject.Professor != null
                        ? g.Enrollment.Subject.Professor.FirstName + " " +
                          g.Enrollment.Subject.Professor.LastName
                        : null,

                    AnnulmentRequested = g.AnnulmentRequested,
                    AnnulmentRequestedAt = g.AnnulmentRequestedAt,

                    CanRequestAnnulment =
                        !g.AnnulmentRequested &&
                        (now - g.AssignedAt).TotalDays <= 3
                })
                .OrderByDescending(g => g.AssignedAt)
                .ToListAsync();

            return grades;
        }

        public async Task<Grade> RequestAnnulment(int gradeId, int userId)
        {
            var g = await _databaseContext.Grades
                .Include(x => x.Enrollment)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(x => x.Id == gradeId)
                ?? throw new KeyNotFoundException("Grade not found");

            // ✅ Ownership check: grade must belong to the logged-in student's UserId
            var ownerUserId = g.Enrollment?.Student?.UserId;
            if (ownerUserId == null || ownerUserId.Value != userId)
                throw new UnauthorizedAccessException("You can only request annulment for your own grade.");

            var within3Days = (DateTimeOffset.UtcNow - g.AssignedAt).TotalDays <= 3;
            if (!within3Days)
                throw new InvalidOperationException("Annulment window (3 days) has expired.");

            if (g.AnnulmentRequested) return g;

            g.AnnulmentRequested = true;
            g.AnnulmentRequestedAt = DateTimeOffset.UtcNow;
            await _databaseContext.SaveChangesAsync();
            return g;
        }
    }
}