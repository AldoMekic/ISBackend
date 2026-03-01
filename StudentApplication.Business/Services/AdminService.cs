using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public AdminService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task CreateAdmin(AdminRequestDTO model)
        {
            if (await _databaseContext.Admins.AnyAsync(a => a.Username == model.Username))
                throw new InvalidOperationException("Admin username must be unique.");

            var admin = _mapper.Map<Admin>(model);
            await _databaseContext.Admins.AddAsync(admin);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Admin?>> GetAll()
        {
            return await _databaseContext.Admins.AsNoTracking().ToListAsync();
        }

        public async Task<Admin> GetById(int id)
        {
            var result = await _databaseContext.Admins.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null) throw new KeyNotFoundException("Admin not found");
            return result;
        }

        public async Task RemoveAdmin(Admin admin)
        {
            _databaseContext.Admins.Remove(admin);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAdmin(Admin admin)
        {
            _databaseContext.Update(admin);
            await _databaseContext.SaveChangesAsync();
        }
    }
}