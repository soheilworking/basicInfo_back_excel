using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.API.Features.Logs.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;
using FormFile = ORG.BasicInfo.API.Features.Forms.Queries.TResponse.FormFile;
namespace ORG.BasicInfo.API.Features.Logs.Queries;

public class GetLogFormByIdRequestHandler : IRequestHandler<GetLogFormByIdRequest, Result<LogFormInfoResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetLogFormByIdRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetLogFormByIdRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetLogFormByIdRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<LogFormInfoResponse>> Handle(GetLogFormByIdRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<LogFormInfoResponse>.Invalid(validationResult.AsErrors());
        }
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var IpUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();

        if ((bool)IsRoleOrg == true)
        {       
            var results = await getItems.GetInfoWithId(Guid.Parse(request.Id), _dbContext, cancellationToken);
            if (results == null) return Result<LogFormInfoResponse>.NotFound("آیتم مورد نظر یافت نشد و یا حذف گردیده است.");

       
            return Result<LogFormInfoResponse>.Success(results);
        }
        else if((bool)IsRoleOrg == false){


            var results = await getItems.GetInfoRejectWithIdForUser(Guid.Parse(request.Id),userId, _dbContext, cancellationToken);
            if(results==null) return Result<LogFormInfoResponse>.NotFound("آیتم مورد نظر یافت نشد و یا حذف گردیده است.");
             
           

            return Result<LogFormInfoResponse>.Success(results);

        }
        
        
        return Result<LogFormInfoResponse>.Unauthorized("دسترسی غیر مجاز");
    }
}