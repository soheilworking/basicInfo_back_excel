using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Users.Queries;
public class GetRepeatedUserWithFundCodeRequest :
    IRequest<Result<RepeatedResult>>
{
    public GetRepeatedUserWithFundCodeRequest(ulong fundCode,string id)
    {
        FundCode = fundCode;
        Id = id;
    }
    public ulong FundCode { get; set; }
    public string Id { get; set; }
}
