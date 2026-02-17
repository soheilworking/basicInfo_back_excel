using FluentValidation;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class RemoveFileRequestValidator : AbstractValidator<RemoveFileRequest>
{
    public RemoveFileRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}