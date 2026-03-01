using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IEnrollmentService
    {
        Task<Enrollment> CreateEnrollment(EnrollmentRequestDTO model);
        Task<IEnumerable<Enrollment?>> GetAll();
        Task<Enrollment> GetById(int id);
        Task<Enrollment?> GetByComposite(int studentId, int subjectId);
        Task RemoveEnrollment(Enrollment enrollment);
        Task UpdateEnrollment(Enrollment enrollment);

        Task<Enrollment> SetStatus(int id, EnrollmentStatus status);
        Task<Enrollment> Drop(int id);
        Task<Enrollment> Complete(int id, DateTimeOffset? completedAt = null);

        Task<IReadOnlyList<Enrollment>> GetForStudentUserId(int userId);
    }
}
