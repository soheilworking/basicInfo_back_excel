using FluentValidation;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries
{
    public class GetRepeatedFormUserWithIdCodeRequestValidator :
        AbstractValidator<GetRepeatedFormUserWithIdCodeRequest>
    {
        public GetRepeatedFormUserWithIdCodeRequestValidator()
        {
            RuleFor(request => request.Id)
            .NotEmpty()
            .Must(id => id==null|| Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");


            RuleFor(request => request.IdCode)
               .NotEmpty()
               .WithMessage("لطفا کد صندوق را وارد نمایید.");

        }
    }
}
