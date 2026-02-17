using FluentValidation;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class ChangeAcceptFormRequestValidator : AbstractValidator<ChangeAcceptFormRequest>
{
    public ChangeAcceptFormRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}