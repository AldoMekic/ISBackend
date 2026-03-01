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
    public class EnrollmentMappingProfile : Profile
    {
        public EnrollmentMappingProfile()
        {
            CreateMap<EnrollmentRequestDTO, Enrollment>();

            CreateMap<Enrollment, EnrollmentResponseDTO>()
                .ForMember(d => d.Subject, opt => opt.MapFrom(s => s.Subject))
                .ForMember(d => d.Grade, opt => opt.MapFrom(s => s.Grade))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (EnrollmentStatusDTO)s.Status));
        }
    }
}
