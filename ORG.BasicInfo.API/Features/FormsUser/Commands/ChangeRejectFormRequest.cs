using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands
{
    public record ChangeRejectFormRequest(string Id,string Description) : IRequest<Result<ResponseWrite>>;
    
}
