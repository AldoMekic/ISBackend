using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int? YearOfStudy { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<Grade> Grades { get; set; } = new List<Grade>();
    }
}
