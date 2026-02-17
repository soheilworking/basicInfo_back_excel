using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.Forms.Commands
{
    public record DeleteFormRequest(string Id) : IRequest<Result<ResponseWrite>>;
    
}
