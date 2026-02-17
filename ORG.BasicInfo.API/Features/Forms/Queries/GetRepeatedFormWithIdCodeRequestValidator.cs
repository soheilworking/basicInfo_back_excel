using FluentValidation;
namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetRepeatedFormWithIdCodeRequestValidator :
        AbstractValidator<GetRepeatedFormWithIdCodeRequest>
    {
        public GetRepeatedFormWithIdCodeRequestValidator()
        {
            RuleFor(request => request.Id)
            .Must(id => id==null|| Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");


            RuleFor(request => request.IdCode)
               .NotEmpty()
               .WithMessage("لطفا کد صندوق را وارد نمایید.");

        }
    }
}
