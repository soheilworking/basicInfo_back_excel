using FluentValidation;
namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetRepeatedUserWithMobileRequestValidator :
        AbstractValidator<GetRepeatedUserWithMobileRequest>
    {
        public GetRepeatedUserWithMobileRequestValidator()
        {
            RuleFor(request => request.Id)
            .Must(id => id==null|| Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");
            
            RuleFor(request => request.Mobile)
            .NotEmpty()
            .Must(mobile => mobile.ToString().Length==10)
            .WithMessage("شماذه همراه معتبر وارد نمایید");
        }
    }
}
