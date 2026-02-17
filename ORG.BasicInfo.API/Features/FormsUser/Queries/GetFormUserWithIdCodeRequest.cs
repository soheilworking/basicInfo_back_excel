using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetFormUserWithIdCodeRequest :
    IRequest<Result<IEnumerable<FormUserListResponse>>>
{
    public GetFormUserWithIdCodeRequest( ulong idCode, string readStatus)
    {
        IdCode = idCode;
        ReadStatus = readStatus;

    }
    public ulong IdCode { get; set; }
    public string ReadStatus { get; set; }
}
