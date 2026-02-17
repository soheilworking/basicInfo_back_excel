using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands
{
    public record ChangeAcceptFormRequest(string Id) : IRequest<Result<ResponseWrite>>;
    
}
