using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public StudentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task EnrollStudentInSubject(int studentId, int subjectId)
        {
            // validate
            if (!await _databaseContext.Students.AnyAsync(s => s.Id == studentId))
                throw new KeyNotFoundException("Student not found");
            if (!await _databaseContext.Subjects.AnyAsync(s => s.Id == subjectId))
                throw new KeyNotFoundException("Subject not found");

            var exists = await _databaseContext.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);
            if (exists) return;

            await _databaseContext.Enrollments.AddAsync(new Enrollment
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Status = EnrollmentStatus.Attending
            });
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<Subject>> GetStudentSubjects(int studentId)
        {
            return await _databaseContext.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Subject)
                .Select(e => e.Subject)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UnenrollStudentFromSubject(int studentId, int subjectId)
        {
            var enrollment = await _databaseContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);

            if (enrollment == null) return;

            _databaseContext.Enrollments.Remove(enrollment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task CreateStudent(StudentRequestDTO model)
        {
            var student = _mapper.Map<Student>(model);
            await _databaseContext.Students.AddAsync(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student?>> GetAll()
        {
            return await _databaseContext.Students
                .AsNoTracking()
                .Include(u => u.Grades)
                .Include(u => u.Enrollments)
                .ToListAsync();
        }

        public async Task<Student> GetById(int id)
        {
            var result = await _databaseContext.Students
                .Include(u => u.Grades)
                .Include(u => u.Enrollments).ThenInclude(e => e.Subject)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (result == null) throw new KeyNotFoundException("Student not found");
            return result;
        }

        public async Task<Student> GetByName(string name)
        {
            var result = await _databaseContext.Students
                .Include(u => u.Grades)
                .Include(u => u.Enrollments)
                .FirstOrDefaultAsync(a => a.FirstName == name);

            if (result == null) throw new KeyNotFoundException("Student not found");
            return result;
        }

        public async Task<Student> GetFirst()
        {
            var result = await _databaseContext.Students
                .Include(u => u.Grades)
                .Include(u => u.Enrollments)
                .FirstOrDefaultAsync();
            if (result == null) throw new InvalidOperationException("No students in database");
            return result;
        }

        public async Task<List<Grade>> GetStudentGrades(int studentId)
        {
            var student = await _databaseContext.Students
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) throw new KeyNotFoundException("Student not found");
            return student.Grades;
        }

        public async Task RemoveStudent(Student student)
        {
            _databaseContext.Students.Remove(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task RemoveStudentGrade(int studentId, int gradeId)
        {
            var grade = await _databaseContext.Grades.FirstOrDefaultAsync(g => g.Id == gradeId);
            if (grade == null) return;

            // only allow removal if the grade belongs to one of student's enrollments
            var enrollment = await _databaseContext.Enrollments.FirstOrDefaultAsync(e => e.Id == grade.EnrollmentId);
            if (enrollment == null || enrollment.StudentId != studentId) return;

            _databaseContext.Grades.Remove(grade);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateStudent(Student student)
        {
            _databaseContext.Update(student);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<Student> GetByUserId(int userId)
        {
            var student = await _databaseContext.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null) throw new KeyNotFoundException("Student profile not found for this user.");
            return student;
        }
    }
}
