using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;

namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public record DownloadFileRequest(string Id) : IRequest<Result<FileResponse>>;
    
}
