using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetRepeatedFormUserWithIdCodeRequest :
    IRequest<Result<RepeatedResult>>
{
    public GetRepeatedFormUserWithIdCodeRequest(ulong idCode,string id)
    {
        IdCode = idCode;
        Id = id;
    }
    public ulong IdCode { get; set; }
    public string Id { get; set; }
}
