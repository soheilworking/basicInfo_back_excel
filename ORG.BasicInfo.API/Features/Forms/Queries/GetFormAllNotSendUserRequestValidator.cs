using FluentValidation;


namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetFormAllNotSendUserRequestValidator :
        AbstractValidator<GetFormAllNotSendUserRequest>
    {
        public GetFormAllNotSendUserRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
