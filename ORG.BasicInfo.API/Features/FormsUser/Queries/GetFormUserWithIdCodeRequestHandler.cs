using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class GetFormUserWithIdCodeRequestHandler : IRequestHandler<GetFormUserWithIdCodeRequest, Result<IEnumerable<FormUserListResponse>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormUserWithIdCodeRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormUserWithIdCodeRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormUserWithIdCodeRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<IEnumerable<FormUserListResponse>>> Handle(GetFormUserWithIdCodeRequest request, CancellationToken cancellationToken)
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
            return Result<IEnumerable<FormUserListResponse>>.Invalid(validationResult.AsErrors());
        }
        FormUserListResponse results = null;
        if (IsRoleOrg == null)
        {
            return Result<IEnumerable<FormUserListResponse>>.Unauthorized("دسترسی غیر مجاز");

        }

        else if ((ushort)IsRoleOrg == 1)
        {
            //ResponseQuery<FormUserListResponse> results = null;
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
            
                    results = await getItems.GetInfoWithIdCode(request.IdCode, _dbContext, cancellationToken);

                    break;
            }
            if (results == null)
                results = await getItems.GetInfoWithIdCodeStateAction(request.IdCode,
                    action,
                    _dbContext,
                    cancellationToken);


        }
        else if ((ushort)IsRoleOrg >1)
        {


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

                    results = await getItems.GetInfoWithIdCodeForUser(request.IdCode, fundsList, _dbContext, cancellationToken);

                    break;
            }
            if (results == null)
                results = await getItems.GetInfoWithIdCodeForUserStateAction(
                    request.IdCode,
                    fundsList,
                    action,
                    _dbContext,
                    cancellationToken);

        }
        return Result<IEnumerable<FormUserListResponse>>.Success(results != null ? [results] : null);
    }
}