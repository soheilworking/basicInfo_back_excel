using FluentValidation;

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class UpsertUserRequestValidator : AbstractValidator<UpsertUserRequest>
{
    public UpsertUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(40);
        RuleFor(request => request.Password)
     .MaximumLength(30).WithMessage("حداکثر طول رمز عبور ۳۰ کاراکتر است")
     .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
         .When(x => !string.IsNullOrEmpty(x.Password))
         .WithMessage("رمز عبور باید حداقل ۸ کاراکتر و شامل حروف بزرگ، کوچک، عدد و کاراکتر خاص باشد");

        RuleFor(request => request.FundName)
        .NotEmpty()
        .MaximumLength(80);

        RuleFor(request => request.FundCode)
        .NotEmpty();

        RuleFor(request => request.NationalCodeUser)
       .NotEmpty();

        RuleFor(request => request.IdCodeNationalFund)
        .NotEmpty();

        RuleFor(request => request.Mobile)
            .NotEmpty()
            .Must(mobile => mobile >= 1000000000 && mobile <= 9999999999)
            .WithMessage("شماره موبایل باید دقیقا ۹ رقم باشد.");



        //RuleFor(request => request.IsOrgUser)
        //    .Must(isOrg => isOrg == false || isOrg == true);



        RuleFor(request => request.Id)
        .Must(id => id == null || Guid.TryParse(id.ToString(), out _))
        .WithMessage("شناسه باید یک Guid معتبر باشد.");
    }
}