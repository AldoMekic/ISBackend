using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsStudent { get; set; }
        public bool IsProfessor { get; set; }

        public bool IsApproved { get; set; }
        public bool IsAdmin { get; set; }
    }
}
