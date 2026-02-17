using FluentValidation;

namespace ORG.BasicInfo.API.Features.Authentication;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        RuleFor(request => request.FundCode)
            .NotEmpty();

        RuleFor(request => request.VerifyCode)
            .NotEmpty();

    }
}