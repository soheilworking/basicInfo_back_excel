using FluentValidation;


namespace ORG.BasicInfo.API.Features.Logs.Queries
{
    public class GetLogsFormAllRequestValidator :
        AbstractValidator<GetLogsFormAllRequest>
    {
        public GetLogsFormAllRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
