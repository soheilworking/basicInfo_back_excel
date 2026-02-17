using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class GetFormUserWithNameTextRequestHandler : IRequestHandler<GetFormUserWithNameTextRequest, Result<FormUserResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormUserWithNameTextRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormUserWithNameTextRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormUserWithNameTextRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormUserResponse>> Handle(GetFormUserWithNameTextRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (!validationResult.IsValid)
        {
            return Result<Result<FormUserResponse>>.Invalid(validationResult.AsErrors());
        }
        if (IsRoleOrg == null)
        {
            return Result<FormUserResponse>.Unauthorized("دسترسی غیر مجاز");

        }
        else if ((ushort)IsRoleOrg == 1)
        {
            ResponseQuery<FormUserListResponse> results = null;

            Domain.FormUserAggregate.StateAction action = Domain.FormUserAggregate.StateAction.NoRead;
            switch (request.ReadStatus)
            {
                case "read":
                    action = Domain.FormUserAggregate.StateAction.Read;
                    break;

                case "noread":
                    action = Domain.FormUserAggregate.StateAction.NoRead;
                    break;

                case "accept":
                    action = Domain.FormUserAggregate.StateAction.Accept;
                    break;

                case "reject":
                    action = Domain.FormUserAggregate.StateAction.Reject;
                    break;
                default:

                    results = await getItems.SearchWithNameLike(request.Name, request.PageNumber, request.PageSize, request.SortField, _dbContext, cancellationToken);

                    break;
            }
            if (results == null)
                results = await getItems.SearchWithNameLikeStateAction(request.Name, request.PageNumber, request.PageSize, action, request.SortField, _dbContext, cancellationToken);

            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });

        }
        else if ((ushort)IsRoleOrg > 1)
        {
            ResponseQuery<FormUserListResponse> results = null;

            Domain.FormUserAggregate.StateAction action = Domain.FormUserAggregate.StateAction.NoRead;
            switch (request.ReadStatus)
            {
                case "read":
                    action = Domain.FormUserAggregate.StateAction.Read;
                    break;

                case "noread":
                    action = Domain.FormUserAggregate.StateAction.NoRead;
                    break;

                case "accept":
                    action = Domain.FormUserAggregate.StateAction.Accept;
                    break;

                case "reject":
                    action = Domain.FormUserAggregate.StateAction.Reject;
                    break;
                default:

                    results = await getItems.SearchWithNameLikeForUser(request.Name,
                        fundsList,
                        request.PageNumber,
                        request.PageSize,
                        request.SortField,
                        _dbContext,
                        cancellationToken);

                    break;
            }
            if (results == null)
                results = await getItems.SearchWithNameLikeForUserStateAction(request.Name,
                    fundsList,
                    request.PageNumber,
                    request.PageSize,
                    request.SortField,
                    action,
                    _dbContext,
                    cancellationToken);

            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });

        }

        return Result<FormUserResponse>.Unauthorized("دسترسی غیر مجاز");

    }
}