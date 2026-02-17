using FluentValidation;
namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetFormByIdRequestValidator :
        AbstractValidator<GetFormByIdRequest>
    {
        public GetFormByIdRequestValidator()
        {
            RuleFor(request => request.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");

        }
    }
}
