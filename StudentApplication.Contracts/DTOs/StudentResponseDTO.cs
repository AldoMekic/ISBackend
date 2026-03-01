using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentResponseDTO? Department { get; set; }

        public List<EnrollmentResponseDTO> Enrollments { get; set; } = new List<EnrollmentResponseDTO>();

        public List<GradeResponseDTO> Grades { get; set; }
    }
}
