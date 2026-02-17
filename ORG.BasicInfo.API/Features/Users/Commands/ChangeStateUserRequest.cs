using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.Users.Commands
{
    public record ChangeStateUserRequest(string Id) : IRequest<Result<ResponseWrite>>;
    
}
