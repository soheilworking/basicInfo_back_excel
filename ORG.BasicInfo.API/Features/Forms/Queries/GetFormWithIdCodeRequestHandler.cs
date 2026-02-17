using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ORG.BasicInfo.API.Features.Forms.Queries;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;

using ORG.BasicInfo.Data;
using System.Security.Claims;


namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetFormWithIdCodeRequestHandler : IRequestHandler<GetFormWithIdCodeRequest, Result<IEnumerable<FormListResponse>>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormWithIdCodeRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormWithIdCodeRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormWithIdCodeRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<IEnumerable<FormListResponse>>> Handle(GetFormWithIdCodeRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IEnumerable<FormListResponse>>.Invalid(validationResult.AsErrors());
        }
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        if ((bool)IsRoleOrg == false)
        {
            var results = await getItems.GetInfoWithIdCodeForUser(request.IdCode,userId, _dbContext, cancellationToken);
            return Result<IEnumerable<FormListResponse>>.Success(results != null ? [results] : null);
        }
        else if((bool)IsRoleOrg == true)
        {
            var results = await getItems.GetInfoWithIdCode(request.IdCode, _dbContext, cancellationToken);
            return Result<IEnumerable<FormListResponse>>.Success(results != null ? [results] : null);

        }else  return Result<IEnumerable<FormListResponse>>.Unauthorized("دسترسی غیر مجاز");

       
    }
}