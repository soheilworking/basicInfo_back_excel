using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Logs.Queries;
public class GetLogFormByIdRequest :
    IRequest<Result<LogFormInfoResponse>>
{
    public GetLogFormByIdRequest(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
