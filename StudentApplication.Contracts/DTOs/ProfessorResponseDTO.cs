using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class ProfessorResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public string? Title { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentResponseDTO? Department { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public int? ApprovedByAdminId { get; set; }

        public List<SubjectResponseDTO> Subjects { get; set; }
    }
}
