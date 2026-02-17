using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;
namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetFormWithTitleOIdCodeForUserRequestHandler : IRequestHandler<GetFormWithTitleOIdCodeForUserRequest, Result<IEnumerable<FormListResponse>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly IValidator<GetFormWithTitleOIdCodeForUserRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    GetItems getItems;
    public GetFormWithTitleOIdCodeForUserRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormWithTitleOIdCodeForUserRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
        getItems = new GetItems();
    }
    public async Task<Result<IEnumerable < FormListResponse>>> Handle(GetFormWithTitleOIdCodeForUserRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IEnumerable < FormListResponse>>.Invalid(validationResult.AsErrors());
        }
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }

        if (IsRoleOrg == null)
        {
            return Result<IEnumerable<FormListResponse>>.Unauthorized("دسترسی غیر مجاز");
        }
        IEnumerable<FormListResponse> results;
        long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (request.Id != null)
        {
            results = await getItems.GetFormWithIdOptionsForUser(
                fundsList,
                Guid.Parse(request.Id),
                _dbContext, cancellationToken);

        }
        else if (request.Title != null)
            results = await getItems.GetFormWithTitleOptionsForUser(fundsList,
                request.Title,
                _dbContext,
                cancellationToken);
        else if (request.IdCode != null)
        {
            results = await getItems.GetFormWithIdCodeOptionsForUser(fundsList,
                request.IdCode,
                _dbContext,
                cancellationToken);
        }
        else
        {
            results = null;
        }
            //IdsForm
            return Result<IEnumerable<FormListResponse>>.Success(results == null?[]:results);
    }
}