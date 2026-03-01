using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public DepartmentService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task CreateDepartment(DepartmentRequestDTO model)
        {
            if (await _databaseContext.Departments.AnyAsync(d => d.Name == model.Name))
                throw new InvalidOperationException("Department name must be unique.");

            var department = _mapper.Map<Department>(model);
            await _databaseContext.Departments.AddAsync(department);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Department?>> GetAll()
        {
            return await _databaseContext.Departments.AsNoTracking().ToListAsync();
        }

        public async Task<Department> GetById(int id)
        {
            var result = await _databaseContext.Departments.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null) throw new KeyNotFoundException("Department not found");

            return result;
        }

        public async Task RemoveDepartment(Department department)
        {
            _databaseContext.Departments.Remove(department);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateDepartment(Department department)
        {
            _databaseContext.Update(department);
            await _databaseContext.SaveChangesAsync();
        }
    }
}