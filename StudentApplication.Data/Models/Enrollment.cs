using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public DateTimeOffset EnrolledAt { get; set; } = DateTimeOffset.UtcNow;
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Attending;
        public DateTimeOffset? CompletedAt { get; set; }

        public Grade? Grade { get; set; }
    }

    public enum EnrollmentStatus
    {
        Attending = 0,
        Completed = 1,
        Dropped = 2
    }
}
