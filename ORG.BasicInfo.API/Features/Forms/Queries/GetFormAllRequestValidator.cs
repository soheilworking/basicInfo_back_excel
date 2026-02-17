using FluentValidation;


namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetFormAllRequestValidator :
        AbstractValidator<GetFormAllRequest>
    {
        public GetFormAllRequestValidator()
        {

            RuleFor(request => request.PageNumber)
            .NotEmpty();
            RuleFor(request => request.PageSize)
           .NotEmpty();
        }
    }
}
