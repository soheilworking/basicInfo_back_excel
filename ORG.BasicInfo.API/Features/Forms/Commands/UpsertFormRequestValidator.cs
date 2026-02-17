using FluentValidation;

namespace ORG.BasicInfo.API.Features.Forms.Commands;

public class UpsertFormRequestValidator : AbstractValidator<UpsertFormRequest>
{
    public UpsertFormRequestValidator()
    {

        RuleFor(request => request.Id)
        .Must(id => id == null || Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(2000);


        RuleFor(request => request.IdCode)
            .NotEmpty()
            .Must(mobile => mobile >0)
            .WithMessage("شماره فرم باید عدد بزرگتر از 1 باشد");



        RuleFor(request => request.IsPublicForm)
            .Must(isOrg => isOrg == false || isOrg == true);

     

    }
}