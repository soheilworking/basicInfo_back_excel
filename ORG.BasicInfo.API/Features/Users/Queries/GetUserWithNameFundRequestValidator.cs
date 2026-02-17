using FluentValidation;
namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetUserWithNameFundRequestValidator :
        AbstractValidator<GetUserWithNameFundRequest>
    {
        public GetUserWithNameFundRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
