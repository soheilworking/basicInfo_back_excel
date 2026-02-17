using FluentValidation;


namespace ORG.BasicInfo.API.Features.City.Queries
{
    public class GetUserAllRequestValidator :
        AbstractValidator<GetUserAllRequest>
    {
        public GetUserAllRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
