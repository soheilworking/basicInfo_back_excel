using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public record DownloadFileRequest(string Id) : IRequest<Result<FileResponse>>;
    
}
