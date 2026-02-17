using FluentValidation;
namespace ORG.BasicInfo.API.Features.Logs.Queries
{
    public class GetLogFormWithNameTextRequestValidator :
        AbstractValidator<GetLogFormWithNameTextRequest>
    {
        public GetLogFormWithNameTextRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
