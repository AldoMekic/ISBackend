using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class GradeResponseDTO
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int OfficialGrade { get; set; }
        public float TotalScore { get; set; }
        public DateTimeOffset AssignedAt { get; set; }

        // Enriched
        public string? SubjectName { get; set; }
        public string? StudentName { get; set; }
        public string? ProfessorName { get; set; }

        // Annulment UX
        public bool AnnulmentRequested { get; set; }
        public DateTimeOffset? AnnulmentRequestedAt { get; set; }
        public bool CanRequestAnnulment { get; set; }  // computed
    }
}
