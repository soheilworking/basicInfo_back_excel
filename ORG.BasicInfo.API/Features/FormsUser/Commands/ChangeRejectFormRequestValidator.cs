using FluentValidation;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class ChangeRejectFormRequestValidator : AbstractValidator<ChangeRejectFormRequest>
{
    public ChangeRejectFormRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}