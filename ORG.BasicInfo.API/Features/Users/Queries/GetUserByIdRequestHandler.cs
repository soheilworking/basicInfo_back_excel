using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using ORG.BasicInfo.API.Features.Users.Queries;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
namespace ORG.BasicInfo.API.Features.City.Queries;

public class GetUserByIdRequestHandler : IRequestHandler<GetUserByIdRequest, Result<UserInfoResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetUserByIdRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserByIdRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetUserByIdRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<Result<UserInfoResponse>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (ushort)IsRoleOrg != 1)
        {
            return Result<UserInfoResponse>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UserInfoResponse>.Invalid(validationResult.AsErrors());
        }
        var results = await getItems.GetInfoWithId(Guid.Parse(request.Id), _dbContext, cancellationToken);
     
           return Result<UserInfoResponse>.Success(results);
    }
}