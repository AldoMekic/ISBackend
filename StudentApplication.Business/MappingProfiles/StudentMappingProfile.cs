using AutoMapper;
using StudentApplication.Contracts.DTOs;
using StudentApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Business.MappingProfiles
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<StudentRequestDTO, Student>();

            CreateMap<Student, StudentResponseDTO>()
                .ForMember(d => d.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(d => d.Enrollments, opt => opt.MapFrom(s => s.Enrollments))
                .ForMember(d => d.Grades, opt => opt.MapFrom(s => s.Grades));
        }
    }
}
