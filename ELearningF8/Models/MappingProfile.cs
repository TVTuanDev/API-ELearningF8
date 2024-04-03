using AutoMapper;
using ELearningF8.Data;
using ELearningF8.ViewModel;

namespace ELearningF8.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserVM, User>(); // Ánh xạ từ UserVM đến User
        }
    }
}