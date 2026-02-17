using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetUserWithMobileRequest :
    IRequest<Result<IEnumerable<UserListResponse>>>
{
    public GetUserWithMobileRequest( ulong mobile)
    {
        Mobile = mobile;
    }
    public ulong Mobile { get; set; }
}
