using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
namespace ORG.BasicInfo.API.Features.City.Queries;

public class GetRepeatedUserWithMobileRequestHandler : IRequestHandler<GetRepeatedUserWithMobileRequest, Result<RepeatedResult>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetRepeatedUserWithMobileRequest> _validator;
    public GetRepeatedUserWithMobileRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetRepeatedUserWithMobileRequest> validator)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
    }
    public async Task<Result<RepeatedResult>> Handle(GetRepeatedUserWithMobileRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<RepeatedResult>.Invalid(validationResult.AsErrors());
        }

        var results = await getItems.GetIsRpeatedMobile(
            request.Id!=null?Guid.Parse(request.Id):Guid.Empty,
            request.Mobile,
            _dbContext,
            cancellationToken);

           return Result<RepeatedResult>.Success(results);
    }
}