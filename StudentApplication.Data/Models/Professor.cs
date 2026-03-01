using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Professor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string? Title { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool IsApproved { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public int? ApprovedByAdminId { get; set; }
        public Admin? ApprovedByAdmin { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<Grade> GivenGrades { get; set; } = new List<Grade>();
    }
}
