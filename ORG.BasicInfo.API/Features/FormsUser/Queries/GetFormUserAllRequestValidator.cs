using FluentValidation;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public class GetFormUserAllRequestValidator :
        AbstractValidator<GetFormUserAllRequest>
    {
        public GetFormUserAllRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
