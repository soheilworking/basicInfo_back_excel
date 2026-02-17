using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;
public class GetFormUserByIdRequest :
    IRequest<Result<FormUserInfoResponse>>
{
    public GetFormUserByIdRequest(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
