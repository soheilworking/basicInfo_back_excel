using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

using ORG.BasicInfo.Data;


namespace ORG.BasicInfo.API.Features.City.Queries;

public class GetUserWithFundCodeRequestHandler : IRequestHandler<GetUserWithFundCodeRequest, Result<IEnumerable<UserListResponse>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetUserWithFundCodeRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserWithFundCodeRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetUserWithFundCodeRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<Result<IEnumerable<UserListResponse>>> Handle(GetUserWithFundCodeRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (bool)IsRoleOrg == false)
        {
            return Result<IEnumerable<UserListResponse>>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IEnumerable<UserListResponse>>.Invalid(validationResult.AsErrors());
        }
        var results = await getItems.GetInfoWithIdCode(request.FundCode, _dbContext, cancellationToken);

        return Result<IEnumerable<UserListResponse>>.Success(results != null ? [results] : null);
    }
}