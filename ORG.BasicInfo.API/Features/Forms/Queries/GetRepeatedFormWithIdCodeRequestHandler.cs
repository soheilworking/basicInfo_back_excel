using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Data;
namespace ORG.BasicInfo.API.Features.Forms.Queries;

public class GetRepeatedFormWithIdCodeRequestHandler : IRequestHandler<GetRepeatedFormWithIdCodeRequest, Result<RepeatedResult>>
{
    private readonly FormsInfoDbContext _dbContext;
    private readonly GetItems getItems;
    private readonly IValidator<GetRepeatedFormWithIdCodeRequest> _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetRepeatedFormWithIdCodeRequestHandler(FormsInfoDbContext dbContext
        ,IValidator<GetRepeatedFormWithIdCodeRequest> validator,
        IHttpContextAccessor httpContextAccessor)
    {
        getItems = new GetItems();
        _dbContext = dbContext;
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<Result<RepeatedResult>> Handle(GetRepeatedFormWithIdCodeRequest request, CancellationToken cancellationToken)
    {
        var IsRoleOrg = _httpContextAccessor.HttpContext.Items["IsRoleOrg"];
        if (IsRoleOrg == null || (bool)IsRoleOrg == false)
        {
            return Result<RepeatedResult>.Unauthorized("دسترسی غیر مجاز");
        }
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<RepeatedResult>.Invalid(validationResult.AsErrors());
        }

        var results = await getItems.GetIsRpeatedIdCode(
            request.Id!=null?Guid.Parse(request.Id):Guid.Empty,
            request.IdCode,
            _dbContext,
            cancellationToken);

           return Result<RepeatedResult>.Success(results);
    }
}