using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Forms.Queries;
public class GetFormWithIdCodeRequest :
    IRequest<Result<IEnumerable<FormListResponse>>>
{
    public GetFormWithIdCodeRequest( ulong idCode)
    {
        IdCode = idCode;
    }
    public ulong IdCode { get; set; }
}
