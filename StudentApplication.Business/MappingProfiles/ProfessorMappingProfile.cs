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
    public class ProfessorMappingProfile : Profile
    {
        public ProfessorMappingProfile()
        {
            CreateMap<ProfessorRequestDTO, Professor>();

            CreateMap<Professor, ProfessorResponseDTO>()
                .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(d => d.Subjects, opt => opt.MapFrom(s => s.Subjects));
        }
    }
}
