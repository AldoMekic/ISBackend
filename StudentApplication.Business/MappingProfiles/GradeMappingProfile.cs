using AutoMapper;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.MappingProfiles
{
    public class GradeMappingProfile : Profile
    {
        public GradeMappingProfile()
        {
            CreateMap<GradeRequestDTO, Grade>();
            CreateMap<Grade, GradeResponseDTO>()
            .ForMember(d => d.SubjectName, o => o.MapFrom(g => g.Enrollment.Subject.Title))
            .ForMember(d => d.StudentName, o => o.MapFrom(g => g.Enrollment.Student.FirstName + " " + g.Enrollment.Student.LastName))
            .ForMember(d => d.ProfessorName, o => o.MapFrom(g => g.Enrollment.Subject.Professor != null
                                                                ? (g.Enrollment.Subject.Professor.FirstName + " " + g.Enrollment.Subject.Professor.LastName)
                                                                : null))
            // computed (3-day window)
            .ForMember(d => d.CanRequestAnnulment, o => o.MapFrom(g =>
                !g.AnnulmentRequested && (DateTimeOffset.UtcNow - g.AssignedAt).TotalDays <= 3
            ));
        }
    }
}
