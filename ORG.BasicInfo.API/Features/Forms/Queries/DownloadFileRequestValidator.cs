using FluentValidation;

namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class DownloadFileRequestValidator : AbstractValidator<DownloadFileRequest>
{
    public DownloadFileRequestValidator()
    {

        RuleFor(request => request.Id)
        .NotEmpty()
        .Must(id => Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}