using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetFormAllNotSendUserRequestHandler : IRequestHandler<GetFormAllNotSendUserRequest, Result<FormResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormAllNotSendUserRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormAllNotSendUserRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormAllNotSendUserRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormResponse>> Handle(GetFormAllNotSendUserRequest request, CancellationToken cancellationToken)
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
         else if ((ushort)IsRoleOrg == 1)
        {
        
        
            var results = await getItems.GetInfoAllNotSend(request.PageNumber, request.PageSize,request.SortField,_dbContext, cancellationToken);

               return Result<FormResponse>.Success(new FormResponse { ListResponse = results.Items, Count = results.Counts });
           
        }else 
        {
            var results = await getItems.GetInfoAllNotSendForUser(fundsList,
                request.PageNumber,
                request.PageSize,
                request.SortField,
                _dbContext,
                cancellationToken);
            return Result<FormResponse>.Success(new FormResponse { ListResponse = results.Items, Count = results.Counts });

        }

    }
}