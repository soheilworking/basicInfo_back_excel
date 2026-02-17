using FluentValidation;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public class GetFormUserNotSendRequestValidator :
        AbstractValidator<GetFormUserNotSendRequest>
    {
        public GetFormUserNotSendRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
