using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface ISubjectService
    {
        Task CreateSubject(SubjectRequestDTO model);
        Task<IEnumerable<Subject?>> GetAll();
        Task<Subject> GetById(int id);
        Task<Subject> GetByName(string name);
        Task<Subject> GetFirst();
        Task RemoveSubject(Subject subject);
        Task UpdateSubject(Subject subject);
    }
}
