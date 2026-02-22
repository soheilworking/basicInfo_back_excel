using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Security.Claims;
using FormFile = ORG.BasicInfo.API.Features.Forms.Queries.TResponse.FormFile;
namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetFormByIdRequestHandler : IRequestHandler<GetFormByIdRequest, Result<FormInfoResponse>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetFormByIdRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetFormByIdRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetFormByIdRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<FormInfoResponse>> Handle(GetFormByIdRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<FormInfoResponse>.Invalid(validationResult.AsErrors());
        }
        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(claim.Value);
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        var IpUser = _httpContextAccessor.HttpContext.Items["IpUser"].ToString();
        var permissionFunds = _httpContextAccessor.HttpContext.Items["PermissionFunds"];
        IEnumerable<Guid> fundsList = [];
        if (permissionFunds != null)
        {
            fundsList = (IEnumerable<Guid>)permissionFunds;
        }
        if((ushort)IsRoleOrg == 3)
        {
            fundsList = [userId];
        }
        if ((ushort)IsRoleOrg == 1)
        {
            var results = await getItems.GetInfoWithId(Guid.Parse(request.Id), _dbContext, cancellationToken);
            if (results == null) return Result<FormInfoResponse>.NotFound("آیتم مورد نظر یافت نشد و یا حذف گردیده است.");

            results.FilesList = await _dbContext
                .FilesRawSyss
                .Where(item => item.IdFormRaw == Guid.Parse(request.Id))
                .Select(itemFile =>
                new FormFile(itemFile.Id.ToString(),
                itemFile.Title,
                itemFile.UploadDate,
                itemFile.FileSize)).ToArrayAsync();

            var log = new AddLogFormRaw(
                  results.Id,
                  $"دیده شده",
                   Guid.Empty,
                   userId,
                   IpUser,
                  _dbContext);
            await log.SaveChange();
            return Result<FormInfoResponse>.Success(results);
        }
       
        else 
        {


            var results = await getItems.GetInfoWithIdForUser(Guid.Parse(request.Id), fundsList, _dbContext, cancellationToken);
            if (results == null) return Result<FormInfoResponse>.NotFound("آیتم مورد نظر یافت نشد و یا حذف گردیده است.");

            results.FilesList = await _dbContext
                .FilesRawSyss
                .Where(item => (item.IdFormRaw == Guid.Parse(request.Id)))
                .Select(itemFile =>
                new FormFile(itemFile.Id.ToString(),
                itemFile.Title,
                itemFile.UploadDate,
                itemFile.FileSize)).ToArrayAsync();
            var log = new AddLogFormRaw(
              results.Id,
              $"دیده شده",
               userId,
               Guid.Empty,
               IpUser,
              _dbContext);

            await log.SaveChange();

            return Result<FormInfoResponse>.Success(results);

        }
      
    }
}