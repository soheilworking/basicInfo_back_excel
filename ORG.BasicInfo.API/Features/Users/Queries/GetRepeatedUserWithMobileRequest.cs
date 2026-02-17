using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetRepeatedUserWithMobileRequest :
    IRequest<Result<RepeatedResult>>
{
    public GetRepeatedUserWithMobileRequest(ulong mobile,string id)
    {
        Mobile = mobile;
        Id = id;
    }
    public ulong Mobile { get; set; }
    public string Id { get; set; }
}
