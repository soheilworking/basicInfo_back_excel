using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetUserWithFundCodeRequest :
    IRequest<Result<IEnumerable<UserListResponse>>>
{
    public GetUserWithFundCodeRequest( ulong fundCode)
    {
        FundCode = fundCode;
    }
    public ulong FundCode { get; set; }
}
