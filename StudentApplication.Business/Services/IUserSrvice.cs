using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public interface IUserService
    {
        Task CreateUser(UserRegisterRequestDTO model);
        Task<IEnumerable<User?>> GetAll();
        Task<User> GetById(int id);
        Task<User?> GetByUsername(string name);
        Task RemoveUser(User user);
        Task UpdateUser(User user);

        // Existing (kept for compatibility)
        Task<IReadOnlyList<User>> GetUnapprovedProfessors();
        Task ApproveProfessor(int userId, int approvedByAdminId, string approvedByAdminName);

        // ✅ NEW: what the Admin dashboard actually needs (Professor + User + Department)
        Task<IReadOnlyList<Professor>> GetPendingProfessorProfiles();
    }
}