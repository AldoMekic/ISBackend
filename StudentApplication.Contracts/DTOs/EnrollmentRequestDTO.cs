using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Contracts.DTOs
{
    public class EnrollmentRequestDTO
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
    }
}
