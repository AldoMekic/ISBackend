using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public enum EnrollmentStatusDTO { Attending = 0, Completed = 1, Dropped = 2 }

    public class EnrollmentResponseDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public EnrollmentStatusDTO Status { get; set; }
        public DateTimeOffset EnrolledAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }

        public string? SubjectTitle { get; set; }
        public string? StudentName { get; set; }

        public SubjectResponseDTO Subject { get; set; }
        public GradeResponseDTO? Grade { get; set; }
    }
}
