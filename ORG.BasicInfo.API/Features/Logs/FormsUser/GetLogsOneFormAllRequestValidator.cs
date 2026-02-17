using FluentValidation;


namespace ORG.BasicInfo.API.Features.Logs.Queries
{
    public class GetLogsOneFormAllRequestValidator :
        AbstractValidator<GetLogsOneFormAllRequest>
    {
        public GetLogsOneFormAllRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
