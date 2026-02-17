using FluentValidation;
namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetRepeatedUserWithFundCodeRequestValidator :
        AbstractValidator<GetRepeatedUserWithFundCodeRequest>
    {
        public GetRepeatedUserWithFundCodeRequestValidator()
        {
            RuleFor(request => request.Id)
            .Must(id => id==null|| Guid.TryParse(id.ToString(), out _))
            .WithMessage("شناسه باید یک Guid معتبر باشد.");


            RuleFor(request => request.FundCode)
               .NotEmpty()
               .WithMessage("لطفا کد صندوق را وارد نمایید.");

        }
    }
}
