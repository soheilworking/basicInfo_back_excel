using FluentValidation;
namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetFormWithNameTextRequestValidator :
        AbstractValidator<GetFormWithNameTextRequest>
    {
        public GetFormWithNameTextRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
