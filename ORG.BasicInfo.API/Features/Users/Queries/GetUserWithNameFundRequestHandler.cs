using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

using ORG.BasicInfo.Data;


namespace ORG.BasicInfo.API.Features.Users.Queries;

public class GetUserWithNameFundRequestHandler : IRequestHandler<GetUserWithNameFundRequest, Result<UserResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetUserWithNameFundRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserWithNameFundRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetUserWithNameFundRequest> validator,
IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<Result<UserResponse>> Handle(GetUserWithNameFundRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (bool)IsRoleOrg == false)
        {
            return Result<UserResponse>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UserResponse>.Invalid(validationResult.AsErrors());
        }
        var results = await getItems.SearchWithFundNameLike(request.Name,request.PageNumber, request.PageSize,request.SortField,_dbContext, cancellationToken);

           return Result<UserResponse>.Success(new UserResponse { ListResponse = results.Items, Count = results.Counts });
    }
}