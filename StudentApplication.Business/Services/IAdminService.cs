using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.Services
{
    public interface IAdminService
    {
        Task CreateAdmin(AdminRequestDTO model);
        Task<IEnumerable<Admin?>> GetAll();
        Task<Admin> GetById(int id);
        Task RemoveAdmin(Admin admin);
        Task UpdateAdmin(Admin admin);
    }
}
