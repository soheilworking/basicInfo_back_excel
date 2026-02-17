using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class GetFormUserAllRequestHandler : IRequestHandler<GetFormUserAllRequest, Result<FormUserResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormUserAllRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormUserAllRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormUserAllRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormUserResponse>> Handle(GetFormUserAllRequest request, CancellationToken cancellationToken)
    {
        //var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        //var userId = Guid.Parse(claim.Value);
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<FormUserResponse>.Invalid(validationResult.AsErrors());
        }

        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (IsRoleOrg == null)
            return Result<FormUserResponse>.Unauthorized("دسترسی غیر مجاز");

        if ((ushort)IsRoleOrg == 1)
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
                    results = await getItems.GetInfoAll(request.PageNumber,
                        request.PageSize,
                        request.SortField,
                        _dbContext,
                        cancellationToken);

                    break;
            }
            if (results == null)
                results = await getItems.GetInfoAllStateAction(request.PageNumber,
                    request.PageSize,
                    request.SortField,
                    _dbContext,
                    action,
                    cancellationToken);

            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });

          
        }else
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
                    results = await getItems.GetInfoAllForUser(request.PageNumber, request.PageSize, request.SortField, fundsList, _dbContext, cancellationToken);

                    break;
            }
            if(results==null)
                results = await getItems.GetInfoAllForUserStateAction(request.PageNumber,
                    request.PageSize,
                    request.SortField,
                    fundsList,
                    action,
                    _dbContext,
                    cancellationToken);

           


            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });

        }

            
    }
}