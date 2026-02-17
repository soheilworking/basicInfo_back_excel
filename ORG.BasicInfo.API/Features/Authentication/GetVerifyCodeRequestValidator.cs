using FluentValidation;
namespace ORG.BasicInfo.API.Features.Authentication;

public class GetVerifyCodeRequestValidator : AbstractValidator<GetVerifyCodeRequest>
{
    public GetVerifyCodeRequestValidator()
    {
        RuleFor(request => request.FundCode)
            .NotEmpty();
        RuleFor(request => request.Password)
    .NotEmpty();

        RuleFor(request => request.CaptchaCode)
            .NotEmpty();

    }
}