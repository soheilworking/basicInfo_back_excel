using FluentValidation;

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class ChangeStateUserRequestValidator : AbstractValidator<ChangeStateUserRequest>
{
    public ChangeStateUserRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}