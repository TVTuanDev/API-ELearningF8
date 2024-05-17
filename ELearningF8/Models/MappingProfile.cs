using AutoMapper;
using ELearningF8.Data;
using ELearningF8.ViewModel.Post;
using ELearningF8.ViewModel.User;

namespace ELearningF8.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserVM, User>(); // Ánh xạ từ UserVM đến User
            CreateMap<PostVM, Post>();
        }
    }
}