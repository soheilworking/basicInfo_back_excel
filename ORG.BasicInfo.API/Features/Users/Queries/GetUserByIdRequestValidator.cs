using FluentValidation;
namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetUserByIdRequestValidator :
        AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(request => request.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");

        }
    }
}
