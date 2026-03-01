using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class UserRegisterRequestDTO
    {
        // credentials
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // roles
        public bool IsStudent { get; set; }
        public bool IsProfessor { get; set; }

        // profile data
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public int? DepartmentId { get; set; }

        // student only
        public int? YearOfStudy { get; set; }

        // professor only
        public string? Title { get; set; }
    }
}
