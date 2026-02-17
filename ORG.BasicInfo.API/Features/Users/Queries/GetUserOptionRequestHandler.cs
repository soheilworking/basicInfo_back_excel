using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Collections.Generic;


namespace ORG.BasicInfo.API.Features.Users.Queries;

public class GetUserOptionRequestHandler : IRequestHandler<GetUserOptionRequest, Result<IEnumerable< UserListResponse>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetUserOptionRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserOptionRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetUserOptionRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<IEnumerable<UserListResponse>>> Handle(GetUserOptionRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null && (ushort)IsRoleOrg < 1 && (ushort)IsRoleOrg > 2)
        {
            return Result<IEnumerable<UserListResponse>>.Unauthorized("دسترسی غیر مجاز");
        }
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IEnumerable< UserListResponse>>.Invalid(validationResult.AsErrors());
        }
        IEnumerable<UserListResponse> results;
      
        if (request.FundName!=null||request.FundCode==0)
        {      
            if ((ushort)IsRoleOrg == 1)
                results = (await getItems.GetInfoAllOptionsFundName(request.FundName, request.IdsFund.Select(item => Guid.Parse(item)), _dbContext, cancellationToken)).Items;
            else
                results = (await getItems.GetInfoAllOptionsFundNameForUser(request.FundName, fundsList, request.IdsFund.Select(item => Guid.Parse(item)), _dbContext, cancellationToken)).Items;
        }
        else
        {

            if ((ushort)IsRoleOrg == 1)
                results = (await getItems.GetInfoAllOptionsFundCode(request.FundCode, request.IdsFund.Select(item => Guid.Parse(item)), _dbContext, cancellationToken)).Items;
            else
                results = (await getItems.GetInfoAllOptionsFundCodeForUser(request.FundCode, fundsList, request.IdsFund.Select(item => Guid.Parse(item)), _dbContext, cancellationToken)).Items;
        }
        return Result<IEnumerable<UserListResponse>>.Success(results);
    }
}