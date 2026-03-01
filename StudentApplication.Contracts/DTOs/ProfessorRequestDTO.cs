using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class ProfessorRequestDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public string? Title { get; set; }
        public int? DepartmentId { get; set; }
        public int? UserId { get; set; }
    }
}
