using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
namespace ORG.BasicInfo.API.Features.Logs.Queries;
public class GetLogsOneFormUserAllRequest :
    IRequest<Result<IEnumerable<LogFormInfoResponseUser>>>
{
    public GetLogsOneFormUserAllRequest(string id)
    {
        Id =Guid.Parse(id);
    }
    public Guid Id { get; set; }

}
