using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public record DownloadFileUserRequest(string Id) : IRequest<Result<FileUserResponse>>;
    
}
