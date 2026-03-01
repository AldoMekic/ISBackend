using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace StudentApplication.Business.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public UserService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateUser(UserRegisterRequestDTO model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                throw new InvalidOperationException("Username already in use.");

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                throw new InvalidOperationException("Email already in use.");

            // Basic validation for required profile fields (Student/Professor FirstName/LastName/Age are non-nullable)
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || model.Age == null)
                throw new InvalidOperationException("FirstName, LastName and Age are required.");

            if (model.IsStudent && model.DepartmentId == null)
                throw new InvalidOperationException("DepartmentId is required for student registration.");

            if (model.IsProfessor && model.DepartmentId == null)
                throw new InvalidOperationException("DepartmentId is required for professor registration.");

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = Hash(model.Password),

                IsStudent = model.IsStudent,
                IsProfessor = model.IsProfessor,

                // Students auto-approved; professors require approval
                IsApproved = model.IsStudent ? true : !model.IsProfessor,
                IsAdmin = false,

                // approval metadata is set on approval, not registration
                ApprovedAt = null,
                ApprovedByAdminName = null
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); // ensures user.Id

            if (model.IsStudent)
            {
                var student = new Student
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Age = model.Age!.Value,
                    YearOfStudy = model.YearOfStudy,
                    DepartmentId = model.DepartmentId,
                    UserId = user.Id
                };

                await _context.Students.AddAsync(student);
            }

            if (model.IsProfessor)
            {
                var professor = new Professor
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Age = model.Age!.Value,
                    Title = model.Title,
                    DepartmentId = model.DepartmentId,
                    UserId = user.Id,

                    // professor entity approval fields
                    IsApproved = false,
                    ApprovedAt = null,
                    ApprovedByUserId = null
                };

                await _context.Professors.AddAsync(professor);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User?>> GetAll()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            var result = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (result == null) throw new KeyNotFoundException("User not found by ID");
            return result;
        }

        public async Task<User?> GetByUsername(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == name);
        }

        public async Task RemoveUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // uppercase hex
        }

        public async Task<IReadOnlyList<User>> GetUnapprovedProfessors()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsProfessor && !u.IsApproved)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        // ✅ NEW: used by AdminController pending-professor-profiles endpoint
        public async Task<IReadOnlyList<Professor>> GetPendingProfessorProfiles()
        {
            return await _context.Professors
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Department)
                .Where(p => !p.IsApproved)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        // ✅ FIX: approve must update BOTH User and Professor tables
        public async Task ApproveProfessor(int userId, int approvedByAdminId, string approvedByAdminName)
        {
            // Use a transaction because we update multiple tables
            await using var tx = await _context.Database.BeginTransactionAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (!user.IsProfessor) throw new InvalidOperationException("User is not a professor");

            // 1) Approve user
            user.IsApproved = true;
            user.ApprovedAt = DateTime.UtcNow;
            user.ApprovedByAdminName = approvedByAdminName;

            // 2) Approve linked professor entity (if it exists)
            var professor = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professor != null)
            {
                professor.IsApproved = true;
                professor.ApprovedAt = DateTimeOffset.UtcNow;
                professor.ApprovedByUserId = approvedByAdminId > 0 ? approvedByAdminId : null;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}