using FluentValidation;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class ChangeStateFormRequestValidator : AbstractValidator<ChangeStateFormRequest>
{
    public ChangeStateFormRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}