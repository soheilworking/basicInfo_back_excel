using Ardalis.Result;
using MediatR;
namespace ORG.BasicInfo.API.Features.Authentication
{
    public record LogoutRequest() : IRequest<Result<bool>>;
}
