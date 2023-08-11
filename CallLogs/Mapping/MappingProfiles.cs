using AutoMapper;
using Dtos;
using CallLogs.Model;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<CalllogDto, Calllog>();
		CreateMap<Calllog, CalllogDto>();
		CreateMap<UserDto, User>();
		CreateMap<User, UserDto>();
	}
}
