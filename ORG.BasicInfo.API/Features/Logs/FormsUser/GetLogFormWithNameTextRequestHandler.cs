using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Logs.Queries;

public class GetLogFormWithNameTextRequestHandler : IRequestHandler<GetLogFormWithNameTextRequest, Result<LogFormResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetLogFormWithNameTextRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetLogFormWithNameTextRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetLogFormWithNameTextRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<LogFormResponse>> Handle(GetLogFormWithNameTextRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
   
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<LogFormResponse>.Invalid(validationResult.AsErrors());
        }
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (IsRoleOrg == null)
            return Result<LogFormResponse>.Unauthorized("دسترسی غیر مجاز");
        if ((ushort)IsRoleOrg == 1)
        {
            var results = await getItems.SearchWithNameLike(request.Name, request.PageNumber, request.PageSize, request.SortField, _dbContext, cancellationToken);

            return Result<LogFormResponse>.Success(new LogFormResponse { ListResponse = results.Items, Count = results.Counts });

        }
        
        else
        {
            var results = await getItems.SearchWithNameLikeForUser(request.Name,
                fundsList,
                request.PageNumber,
                request.PageSize,
                request.SortField,
                _dbContext,
                cancellationToken);

            return Result<LogFormResponse>.Success(new LogFormResponse { ListResponse = results.Items, Count = results.Counts });

        }
    }
}