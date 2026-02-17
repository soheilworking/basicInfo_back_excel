using FluentValidation;


namespace ORG.BasicInfo.API.Features.Logs.Queries
{
    public class GetLogsOneFormUserAllRequestValidator :
        AbstractValidator<GetLogsOneFormUserAllRequest>
    {
        public GetLogsOneFormUserAllRequestValidator()
        {
            RuleFor(request => request.Id)
          .NotEmpty()
          .Must(id => Guid.TryParse(id.ToString(), out _))
          .WithMessage("شناسه باید یک Guid معتبر باشد.");

        }
    }
}
