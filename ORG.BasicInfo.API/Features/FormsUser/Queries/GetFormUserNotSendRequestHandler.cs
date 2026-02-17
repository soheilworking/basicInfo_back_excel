using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class GetFormUserNotSendRequestHandler : IRequestHandler<GetFormUserNotSendRequest, Result<FormUserResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormUserNotSendRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormUserNotSendRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormUserNotSendRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormUserResponse>> Handle(GetFormUserNotSendRequest request, CancellationToken cancellationToken)
    {
        //var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        //var userId = Guid.Parse(claim.Value);
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if (!validationResult.IsValid)
        {
            return Result<FormUserResponse>.Invalid(validationResult.AsErrors());
        }
        if(IsRoleOrg == null)
            return Result<FormUserResponse>.Unauthorized("دسترسی غیر مجاز");

        if ((ushort)IsRoleOrg ==1)
        {
            ResponseQuery<FormUserListResponse> results = null;

            results = await getItems.GetInfoAllUserNotSend(request.PageNumber, request.PageSize, request.SortField, _dbContext, cancellationToken);

            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });

          
        }else
        {
            
            ResponseQuery<FormUserListResponse> results = null;

            results = await getItems.GetInfoAllUserNotSendForUserOrg(request.PageNumber,
                request.PageSize,
                fundsList,
                request.SortField,
                _dbContext,
                
                cancellationToken);

            return Result<FormUserResponse>.Success(new FormUserResponse { ListResponse = results.Items, Count = results.Counts });



        }

            
    }
}