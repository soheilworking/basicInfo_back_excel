using FluentValidation;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public class GetFormUserWithNameTextRequestValidator :
        AbstractValidator<GetFormUserWithNameTextRequest>
    {
        public GetFormUserWithNameTextRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
