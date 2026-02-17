using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
namespace ORG.BasicInfo.API.Features.City.Queries;

public class GetRepeatedUserWithFundCodeRequestHandler : IRequestHandler<GetRepeatedUserWithFundCodeRequest, Result<RepeatedResult>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetRepeatedUserWithFundCodeRequest> _validator;
    public GetRepeatedUserWithFundCodeRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetRepeatedUserWithFundCodeRequest> validator)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
    }
    public async Task<Result<RepeatedResult>> Handle(GetRepeatedUserWithFundCodeRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<RepeatedResult>.Invalid(validationResult.AsErrors());
        }

        var results = await getItems.GetIsRpeatedIdCode(
            request.Id!=null?Guid.Parse(request.Id):Guid.Empty,
            request.FundCode,
            _dbContext,
            cancellationToken);

           return Result<RepeatedResult>.Success(results);
    }
}