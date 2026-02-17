using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetFormWithNameTextRequestHandler : IRequestHandler<GetFormWithNameTextRequest, Result<FormResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormWithNameTextRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormWithNameTextRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormWithNameTextRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormResponse>> Handle(GetFormWithNameTextRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
   
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<FormResponse>.Invalid(validationResult.AsErrors());
        }
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (IsRoleOrg == null)
        {
            return Result<FormResponse>.Unauthorized("دسترسی غیر مجاز");
        }
        if ((ushort)IsRoleOrg == 1)
        {
            var results = await getItems.SearchWithNameLike(request.Name, request.PageNumber, request.PageSize, request.SortField, _dbContext, cancellationToken);

            return Result<FormResponse>.Success(new FormResponse { ListResponse = results.Items, Count = results.Counts });
        }else 
        {
            var results = await getItems.SearchWithNameLikeForUser(request.Name, fundsList, request.PageNumber, request.PageSize, request.SortField, _dbContext, cancellationToken);

            return Result<FormResponse>.Success(new FormResponse { ListResponse = results.Items, Count = results.Counts });

        }
    }
}