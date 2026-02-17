using FluentValidation;
using ORG.BasicInfo.API.Features.Users.Queries;

namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetUserWithNamesRequestValidator :
        AbstractValidator<GetUserWithNamesRequest>
    {
        public GetUserWithNamesRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
