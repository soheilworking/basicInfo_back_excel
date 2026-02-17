using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetUserByIdRequest :
    IRequest<Result<UserInfoResponse>>
{
    public GetUserByIdRequest(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
