using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Forms.Queries;
public class GetFormByIdRequest :
    IRequest<Result<FormInfoResponse>>
{
    public GetFormByIdRequest(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
