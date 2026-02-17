using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
namespace ORG.BasicInfo.API.Features.Forms.Queries;
public class GetRepeatedFormWithIdCodeRequest :
    IRequest<Result<RepeatedResult>>
{
    public GetRepeatedFormWithIdCodeRequest(ulong idCode,string id)
    {
        IdCode = idCode;
        Id = id;
    }
    public ulong IdCode { get; set; }
    public string Id { get; set; }
}
