//using AutoMapper;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Users;

public class UserMapperProfile 
{
    public UserMapperProfile()
    {
        //CreateMap<User, UserResponse>(MemberList.Destination);
    }

    public override string ProfileName => nameof(UserMapperProfile);
}