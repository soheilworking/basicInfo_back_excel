using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;

using ORG.BasicInfo.Data;


namespace ORG.BasicInfo.API.Features.City.Queries;

public class GetUserWithNamesRequestHandler : IRequestHandler<GetUserWithNamesRequest, Result<UserResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetUserWithNamesRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserWithNamesRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetUserWithNamesRequest> validator,
IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<Result<UserResponse>> Handle(GetUserWithNamesRequest request, CancellationToken cancellationToken)
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
        var results = await getItems.SearchWithNameLike(request.Name,request.PageNumber, request.PageSize,request.SortField,_dbContext, cancellationToken);

           return Result<UserResponse>.Success(new UserResponse { ListResponse = results.Items, Count = results.Counts });
    }
}