using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data;
using StudentApplication.Data.Models;

namespace StudentApplication.Business.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public SubjectService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task CreateSubject(SubjectRequestDTO model)
        {
            // Make sure the professor exists and is approved
            var professor = await _databaseContext.Professors
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == model.ProfessorId);

            if (professor == null)
                throw new KeyNotFoundException("Professor not found");

            // We need the user record to check IsApproved (assuming Professor ↔ User via Username/Email/UserId).
            // If Professor has a UserId, use that; otherwise adjust accordingly.
            var user = await _databaseContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == professor.UserId);

            if (user == null)
                throw new InvalidOperationException("Professor account record not found");

            if (!user.IsApproved)
                throw new InvalidOperationException("Professor is not approved and cannot be assigned to a subject.");

            var subject = _mapper.Map<Subject>(model);
            await _databaseContext.Subjects.AddAsync(subject);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subject?>> GetAll()
        {
            return await _databaseContext.Subjects
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subject> GetById(int id)
        {
            var result = await _databaseContext.Subjects.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null) throw new KeyNotFoundException("Subject not found");
            return result;
        }

        public async Task<Subject> GetByName(string name)
        {
            var result = await _databaseContext.Subjects.FirstOrDefaultAsync(a => a.Title == name);
            if (result == null) throw new KeyNotFoundException("Subject not found");
            return result;
        }

        public async Task<Subject> GetFirst()
        {
            var result = await _databaseContext.Subjects.FirstOrDefaultAsync();
            if (result == null) throw new InvalidOperationException("No subjects in database");
            return result;
        }

        public async Task RemoveSubject(Subject subject)
        {
            _databaseContext.Subjects.Remove(subject);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateSubject(Subject subject)
        {
            // if ProfessorId changed, validate approval again
            var user = await _databaseContext.Users
                .AsNoTracking()
                .Where(u => u.Id == subject.Professor.UserId)
                .Select(u => new { u.IsApproved })
                .FirstOrDefaultAsync();

            if (user is null) throw new InvalidOperationException("Professor account record not found");
            if (!user.IsApproved) throw new InvalidOperationException("Professor is not approved and cannot be assigned to a subject.");

            _databaseContext.Update(subject);
            await _databaseContext.SaveChangesAsync();
        }
    }
}