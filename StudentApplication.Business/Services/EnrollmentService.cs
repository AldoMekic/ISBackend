using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public EnrollmentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task<Enrollment> CreateEnrollment(EnrollmentRequestDTO model)
        {
            // Validate existence
            var studentExists = await _databaseContext.Students.AnyAsync(s => s.Id == model.StudentId);
            if (!studentExists) throw new KeyNotFoundException("Student not found");

            var subjectExists = await _databaseContext.Subjects.AnyAsync(s => s.Id == model.SubjectId);
            if (!subjectExists) throw new KeyNotFoundException("Subject not found");

            // Uniqueness per (student,subject)
            var already = await _databaseContext.Enrollments.AnyAsync(e => e.StudentId == model.StudentId && e.SubjectId == model.SubjectId);
            if (already) throw new InvalidOperationException("Student is already enrolled in this subject.");

            var enrollment = _mapper.Map<Enrollment>(model);
            await _databaseContext.Enrollments.AddAsync(enrollment);
            await _databaseContext.SaveChangesAsync();

            return enrollment;
        }

        public async Task<IEnumerable<Enrollment?>> GetAll()
        {
            return await _databaseContext.Enrollments
                .AsNoTracking()
                .Include(e => e.Subject).ThenInclude(s => s.Professor)
                .Include(e => e.Student)
                .Include(e => e.Grade)
                .ToListAsync();
        }

        public async Task<Enrollment> GetById(int id)
        {
            var result = await _databaseContext.Enrollments
                .Include(e => e.Subject).ThenInclude(s => s.Professor)
                .Include(e => e.Student)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (result == null) throw new KeyNotFoundException("Enrollment not found");
            return result;
        }

        public async Task<Enrollment?> GetByComposite(int studentId, int subjectId)
        {
            return await _databaseContext.Enrollments
                .Include(e => e.Subject)
                .Include(e => e.Grade)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);
        }

        public async Task RemoveEnrollment(Enrollment enrollment)
        {
            _databaseContext.Enrollments.Remove(enrollment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            _databaseContext.Update(enrollment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<Enrollment> SetStatus(int id, EnrollmentStatus status)
        {
            var e = await _databaseContext.Enrollments.FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new KeyNotFoundException("Enrollment not found");

            e.Status = status;
            if (status == EnrollmentStatus.Completed && e.CompletedAt == null)
                e.CompletedAt = DateTimeOffset.UtcNow;

            await _databaseContext.SaveChangesAsync();
            return e;
        }


        public Task<Enrollment> Drop(int id) => SetStatus(id, EnrollmentStatus.Dropped);

        public Task<Enrollment> Complete(int id, DateTimeOffset? completedAt = null)
        {
            return CompleteInternal(id, completedAt);
        }

        private async Task<Enrollment> CompleteInternal(int id, DateTimeOffset? completedAt)
        {
            var e = await _databaseContext.Enrollments.FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new KeyNotFoundException("Enrollment not found");
            e.Status = EnrollmentStatus.Completed;
            e.CompletedAt = completedAt ?? DateTimeOffset.UtcNow;
            await _databaseContext.SaveChangesAsync();
            return e;
        }

        public async Task<IReadOnlyList<Enrollment>> GetForStudentUserId(int userId)
        {
            return await _databaseContext.Enrollments
                .AsNoTracking()
                .Include(e => e.Subject).ThenInclude(s => s.Professor)
                .Include(e => e.Student)
                .Include(e => e.Grade)
                .Where(e => e.Student.UserId == userId)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }
    }
}