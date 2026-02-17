using FluentValidation;

namespace ORG.BasicInfo.API.Features.FormsUser.Commands;

public class UpsertFormRequestValidator : AbstractValidator<UpsertFormUserRequest>
{
    public UpsertFormRequestValidator()
    {

        RuleFor(request => request.Id)
        .Must(id => id == null || Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");



        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(2000);



        RuleFor(request => request.IdFormRaw)
          .Must(id => id == null || Guid.TryParse(id.ToString(), out _))
          .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}