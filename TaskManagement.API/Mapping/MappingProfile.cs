using AutoMapper;
using TaskManagement.API.Models.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskModel, TaskDto>();
            CreateMap<TaskCreateRequest, TaskModel>()
                .ForMember(dest => dest.UploadFilePath, opt => opt.Ignore());
            CreateMap<UserModel, UserDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year));

            CreateMap<UserCreateRequest, UserModel>()
                .ForMember(dest => dest.ProfilePicPath, opt => opt.Ignore());

            CreateMap<TaskAssignmentModel, TaskAssignmentDto>()
                .ForMember(dest => dest.TaskTitle, opt => opt.MapFrom(src => src.Task.TaskTitle))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Task.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Task.Status))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.Task.DueDate))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<TaskAssignmentRequest, TaskAssignmentModel>();
        }
    }
}
