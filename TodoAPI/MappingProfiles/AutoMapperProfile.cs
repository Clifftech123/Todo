using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TodoAPI.Contracts;
using TodoAPI.Models;

namespace TodoAPI.MappingProfiles
{
   
    public class AutoMapperProfile : Profile
 {
     public AutoMapperProfile()
     {


         // Create a mapping from the  <CreateTodoRequest  to Todo Models
         CreateMap<CreateTodoRequest, Todo>()
             .ForMember(dest => dest.id, opt => opt.Ignore()) // Ignore the Guid property
             .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Ignore the CreatedAt,  property
             .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Ignore the UpdatedAt property


         CreateMap<UpdateTodoRequest, Todo>()
              .ForMember(dest => dest.id, opt => opt.Ignore()) // Ignore the Guid property
             .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Ignore the CreatedAt,  property
             .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Ignore the UpdatedAt property

     }
 }
}