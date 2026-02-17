using FluentValidation;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class RemoveFileUserRequestValidator : AbstractValidator<RemoveFileUserRequest>
{
    public RemoveFileUserRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}