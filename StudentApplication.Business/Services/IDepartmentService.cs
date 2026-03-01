using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IDepartmentService
    {
        Task CreateDepartment(DepartmentRequestDTO model);
        Task<IEnumerable<Department?>> GetAll();
        Task<Department> GetById(int id);
        Task RemoveDepartment(Department department);
        Task UpdateDepartment(Department department);
    }
}
