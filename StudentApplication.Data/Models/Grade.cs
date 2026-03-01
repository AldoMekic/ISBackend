using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int OfficialGrade { get; set; }
        public float TotalScore { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = default!;

        public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;

        public bool AnnulmentRequested { get; set; } = false;
        public DateTimeOffset? AnnulmentRequestedAt { get; set; }
    }
}
