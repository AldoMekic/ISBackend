using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IStudentService
    {
        Task CreateStudent(StudentRequestDTO model);
        Task<IEnumerable<Student?>> GetAll();
        Task<Student> GetById(int id);
        Task<Student> GetByName(string name);
        Task<Student> GetFirst();
        Task RemoveStudent(Student student);
        Task UpdateStudent(Student student);

        Task EnrollStudentInSubject(int studentId, int subjectId);
        Task<List<Subject>> GetStudentSubjects(int studentId);
        Task UnenrollStudentFromSubject(int studentId, int subjectId);

        Task<List<Grade>> GetStudentGrades(int studentId);
        Task RemoveStudentGrade(int studentId, int gradeId);

        Task<Student> GetByUserId(int userId);
    }
}
