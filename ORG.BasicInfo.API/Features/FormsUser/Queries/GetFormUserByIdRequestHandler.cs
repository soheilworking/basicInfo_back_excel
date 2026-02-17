using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse;
using ORG.BasicInfo.Data;
using FormFile = ORG.BasicInfo.API.Features.FormsUser.Queries.TResponse.FormFile;
using ORG.BasicInfo.API.Features.Abstractions;
namespace ORG.BasicInfo.API.Features.FormsUser.Queries;

public class GetFormUserByIdRequestHandler : IRequestHandler<GetFormUserByIdRequest, Result<FormUserInfoResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormUserByIdRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormUserByIdRequestHandler(
        FormsInfoDbContext dbContext,
        IValidator<GetFormUserByIdRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormUserInfoResponse>> Handle(GetFormUserByIdRequest request, CancellationToken cancellationToken)
    {
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var ipUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if ((bool)IsRoleOrg == null)
        {
            return Result<FormUserInfoResponse>.Unauthorized("دسترسی غیر مجاز");
        }
            if ((ushort)IsRoleOrg >1 )
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<FormUserInfoResponse>.Invalid(validationResult.AsErrors());
            }
            var results = await getItems.GetInfoWithIdForUser(Guid.Parse(request.Id), fundsList, _dbContext, cancellationToken);

            results.FilesList = await _dbContext
                .FilesFormsSyss
                .Where(item => item.IdFormUser == Guid.Parse(request.Id))
                .Select(itemFile =>
                new FormFile(itemFile.Id.ToString(),
                itemFile.Title,
                itemFile.UploadDate,
                itemFile.FileSize)).ToArrayAsync(cancellationToken);

            return Result<FormUserInfoResponse>.Success(results);

        } 
            else if((ushort)IsRoleOrg==1) {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<FormUserInfoResponse>.Invalid(validationResult.AsErrors());
            }
            var resultInfo =await  _dbContext.FormUserSyss.FirstOrDefaultAsync(item => item.Id == Guid.Parse(request.Id));
            if (resultInfo == null)
            {
                return Result<FormUserInfoResponse>.NotFound("آیتم مورد نظر پیدا نشد و یا حذف گردیده است.");
            }
        
            var results = await getItems.GetInfoWithId(Guid.Parse(request.Id), _dbContext, cancellationToken);

            results.FilesList = await _dbContext
                .FilesFormsSyss
                .Where(item => item.IdFormUser == Guid.Parse(request.Id))
                .Select(itemFile =>
                new FormFile(itemFile.Id.ToString(),
                itemFile.Title,
                itemFile.UploadDate,
                itemFile.FileSize)).ToArrayAsync(cancellationToken);
            if (resultInfo.StateAction == Domain.FormUserAggregate.StateAction.NoRead)
            {
                resultInfo.ChangeRead(userId);
                resultInfo.ChangeStateAction(Domain.FormUserAggregate.StateAction.Read);
                _dbContext.Update(resultInfo);
                var log = new AddLogFormUser(
                results.Id,
                results.IdFormRaw,
                $"تغییر وضعیت به خوانده شده شده",
                resultInfo.LUserCreate,
                userId,
                ipUser == null ? "" : ipUser,
                Domain.FormUserAggregate.StateAction.Read,
                _dbContext);

                await log.SaveChange();
                await _dbContext.SaveChangesAsync();
            }
    
            return Result<FormUserInfoResponse>.Success(results);
        }
            else
                return Result<FormUserInfoResponse>.Unauthorized("دسترسی غیر مجاز");

    }
}