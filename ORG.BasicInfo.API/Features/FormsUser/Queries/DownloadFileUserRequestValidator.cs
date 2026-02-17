using FluentValidation;

namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class DownloadFileUserRequestValidator : AbstractValidator<DownloadFileUserRequest>
{
    public DownloadFileUserRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}