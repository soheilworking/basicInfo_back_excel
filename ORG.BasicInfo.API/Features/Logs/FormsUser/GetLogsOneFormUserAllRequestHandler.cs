using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.Logs.Queries;

public class GetLogsOneFormUserAllRequestHandler : IRequestHandler<GetLogsOneFormUserAllRequest, Result<IEnumerable<LogFormInfoResponseUser>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetLogsOneFormUserAllRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetLogsOneFormUserAllRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetLogsOneFormUserAllRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<IEnumerable<LogFormInfoResponseUser>>> Handle(GetLogsOneFormUserAllRequest request, CancellationToken cancellationToken)
    {
        //return Result<IEnumerable<LogFormInfoResponseUser>>.Success(new IEnumerable<LogFormInfoResponseUser>());
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IEnumerable<LogFormInfoResponseUser>>.Invalid(validationResult.AsErrors());
        }
  
        bool isAccess =await _dbContext.FormUserSyss.AnyAsync(
            item => item.Id == request.Id && item.IdUser == userId);
        if(isAccess==true){
            var results = await getItems.GetInfoAllForFormUser(request.Id, _dbContext, cancellationToken);

            return Result<IEnumerable<LogFormInfoResponseUser>>.Success(results);
        }
        else
        {
            return Result<IEnumerable<LogFormInfoResponseUser>>.Forbidden("دسترسی غیر مجاز");

        }

        

    }
}