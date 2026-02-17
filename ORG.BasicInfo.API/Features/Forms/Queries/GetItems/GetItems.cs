using Mapster;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Forms.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;

namespace ORG.BasicInfo.API.Features.Forms.Queries
{
    public class GetItems : IGetItems<FormListResponse, FormInfoResponse, FormRawSys, FormsInfoDbContext>
    {

        public override async Task<ResponseQuery<FormListResponse>> GetInfoAll(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Forms = dbContext
                .FormRawSyss
            .AsNoTracking();
           
            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken); 

        }
        public async Task<ResponseQuery<FormListResponse>> GetInfoAllNotSend(
  
        int pageNumber,
        int pageSize,
        IEnumerable<string[]> sortFields,
        FormsInfoDbContext dbContext,
        CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var forms =
               from formRaw in dbContext.FormRawSyss
                   .Include(f => f.UserFund)
                   .ThenInclude(fu => fu.User)
               where formRaw.State == Domain.Abstractions.EntityState.Active &&
                    (formRaw.ExpireDate == 0 || formRaw.ExpireDate > nowUnixTimeSeconds)
                  && (formRaw.IsPublicForm == true)

               join formUser in dbContext.FormUserSyss
                    on formRaw.Id equals formUser.IdFormRaw into frmUser

               from fUser in frmUser.DefaultIfEmpty()
               where fUser == null

               join formLog in dbContext.FormRawLogSyss
                   on formRaw.Id equals formLog.IdForm into log

               //from log in logs   // LEFT JOIN

               select new FormListResponse
               {
                   Id = formRaw.Id,
                   IdCode = formRaw.IdCode,
                   Title = formRaw.Title,
                   ExpireDate = formRaw.ExpireDate,
                   IsPublicForm = formRaw.IsPublicForm,
                   State = formRaw.State,
                   TCreate = formRaw.TCreate,
                   TEdit = formRaw.TEdit,
                   IsStrongRow = log.Count() == 0,
                   IsRead = log.Count() > 0,
               };



            return await paginationData(pageNumber, pageSize, forms.AsNoTracking(), sortFields, cancellationToken);


        }
        public async Task<ResponseQuery<FormListResponse>> GetInfoAllNotSendForUser(
            IEnumerable<Guid> idUser,
            int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var forms =
               from formRaw in dbContext.FormRawSyss
                   .Include(f => f.UserFund)
                   .ThenInclude(fu => fu.User)
               where formRaw.State == Domain.Abstractions.EntityState.Active &&
                    (formRaw.ExpireDate==0|| formRaw.ExpireDate> nowUnixTimeSeconds)
                  && (formRaw.UserFund.Any(u => idUser.Contains(u.IdUser))
                      || formRaw.IsPublicForm == true)
                
               join formUser in dbContext.FormUserSyss
                    on formRaw.Id equals formUser.IdFormRaw into frmUser

                from fUser in frmUser.DefaultIfEmpty()
                    where fUser==null

               join formLog in dbContext.FormRawLogSyss.Where(x => idUser.Contains(x.IdUser))
                   on formRaw.Id equals formLog.IdForm into log

               //from log in logs   // LEFT JOIN

               select new FormListResponse
               {
                   Id = formRaw.Id,
                   IdCode = formRaw.IdCode,
                   Title = formRaw.Title,
                   ExpireDate = formRaw.ExpireDate,
                   IsPublicForm = formRaw.IsPublicForm,
                   State = formRaw.State,
                   TCreate = formRaw.TCreate,
                   TEdit = formRaw.TEdit,
                   IsStrongRow = log.Count()==0,
                   IsRead = log.Count() > 0,
               };
                

       
            return await paginationData(pageNumber, pageSize, forms.AsNoTracking(), sortFields, cancellationToken);
     

        }
        public async Task<ResponseQuery<FormListResponse>> GetInfoAllForUser(
            IEnumerable<Guid> idUser,
            int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext,
            CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var forms =
               from formRaw in dbContext.FormRawSyss
                   .Include(f => f.UserFund)
                   .ThenInclude(fu => fu.User)
               where formRaw.State == Domain.Abstractions.EntityState.Active &&
                    (formRaw.ExpireDate == 0 || formRaw.ExpireDate > nowUnixTimeSeconds)
                  && (formRaw.UserFund.Any(u => idUser.Contains(u.IdUser))
                      || formRaw.IsPublicForm == true)

        
               join formLog in dbContext.FormRawLogSyss.Where(x => idUser.Contains(x.IdUser))
                   on formRaw.Id equals formLog.IdForm into log

               //from log in logs   // LEFT JOIN

               select new FormListResponse
               {
                   Id = formRaw.Id,
                   IdCode = formRaw.IdCode,
                   Title = formRaw.Title,
                   ExpireDate = formRaw.ExpireDate,
                   IsPublicForm = formRaw.IsPublicForm,
                   State = formRaw.State,
                   TCreate = formRaw.TCreate,
                   TEdit = formRaw.TEdit,
                   IsStrongRow = log.Count() == 0,
                   IsRead = log.Count() > 0,
               };



            return await paginationData(pageNumber, pageSize, forms.AsNoTracking(), sortFields, cancellationToken);


        }

        public override async Task<FormInfoResponse> GetInfoWithId(Guid id, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormRawSyss
                     .Include(f => f.UserFund)
                  .ThenInclude(fu => fu.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
    
            var result= Forms.Adapt<FormInfoResponse>();
            result.IdsFund = Forms.UserFund.Select(item => item.IdUser);
            return result;
        }
        public  async Task<FormInfoResponse> GetInfoWithIdForUser(Guid id,Guid idUser, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var Forms = await dbContext.Set<FormRawSys>()
                    .Include(f => f.UserFund)
                  .ThenInclude(fu => fu.User)
                  .Where(item => item.State == Domain.Abstractions.EntityState.Active &&
                  (item.ExpireDate == 0 || item.ExpireDate > nowUnixTimeSeconds))
                  .Where(
                  item =>
                (item.UserFund.Any(itemUser => itemUser.IdUser == idUser) ||
                item.IsPublicForm == true) && item.Id == id
                )
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return Forms.Adapt<FormInfoResponse>();
        }
        public override async Task<FormListResponse> GetInfoWithIdCode(ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Forms = await dbContext.FormRawSyss
           .AsNoTracking()
           .FirstOrDefaultAsync(item => item.IdCode == idCode);
    
              

            return Forms.Adapt<FormListResponse>();
        }
        public async Task<FormListResponse> GetInfoWithIdCodeForUser(ulong idCode,Guid idUser, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var forms =
                await (
                    from formRaw in dbContext.FormRawSyss
                        .Include(f => f.UserFund)
                        .ThenInclude(fu => fu.User)
                    where formRaw.State == Domain.Abstractions.EntityState.Active &&
                         (formRaw.ExpireDate == 0 || formRaw.ExpireDate > nowUnixTimeSeconds)
                       && (formRaw.UserFund.Any(u => u.IdUser == idUser)
                           || formRaw.IsPublicForm == true)
                       && formRaw.IdCode == idCode

                    join formLog in dbContext.FormRawLogSyss.Where(x => x.IdUser == idUser)
                        on formRaw.Id equals formLog.IdForm into logs

                    from log in logs.DefaultIfEmpty()   // LEFT JOIN

                    select new FormListResponse
                    {
                        Id = formRaw.Id,
                        IdCode = formRaw.IdCode,
                        Title = formRaw.Title,
                        ExpireDate = formRaw.ExpireDate,
                        IsPublicForm = formRaw.IsPublicForm,
                        State = formRaw.State,
                        TCreate = formRaw.TCreate,
                        TEdit = formRaw.TEdit,
                        IsStrongRow = log == null,
                        IsRead = log == null,
                    }
                )
                .AsNoTracking()
                .FirstOrDefaultAsync();




            return forms.Adapt<FormListResponse>();
        }

        public override async Task<ResponseQuery<FormListResponse>> SearchWithNameLike(string name, 
            int pageNumber, 
            int pageSize,
             IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Forms = dbContext.FormRawSyss
                  .AsNoTracking()
                  .Where(FormsItem => EF.Functions.Like(FormsItem.Title, $"%{name}%")|| EF.Functions.Like(FormsItem.Description, $"%{name}%"));
             
            return await paginationData(pageNumber, pageSize, Forms, sortFields, cancellationToken);

        }
        public async Task<ResponseQuery<FormListResponse>> SearchWithNameLikeForUser(string name,
         IEnumerable<Guid> idUser,
        int pageNumber,
        int pageSize,
         IEnumerable<string[]> sortFields,
        FormsInfoDbContext dbContext,
        CancellationToken cancellationToken)
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var forms =
          from formRaw in dbContext.FormRawSyss
              .Include(f => f.UserFund)
              .ThenInclude(fu => fu.User)
          where formRaw.State == Domain.Abstractions.EntityState.Active &&
                         (formRaw.ExpireDate == 0 || formRaw.ExpireDate > nowUnixTimeSeconds)
             && (formRaw.UserFund.Any(u => idUser.Contains(u.IdUser)) || formRaw.IsPublicForm)
             && (
                  EF.Functions.Like(formRaw.Title, $"%{name}%") ||
                  EF.Functions.Like(formRaw.Description, $"%{name}%")
                )
             && (formRaw.IsPublicForm || formRaw.UserFund.Any(u => idUser.Contains(u.IdUser)))

          join formLog in dbContext.FormRawLogSyss.Where(x => idUser.Contains(x.IdUser))
              on formRaw.Id equals formLog.IdForm into logs

          from log in logs.DefaultIfEmpty()   // LEFT JOIN

          select new FormListResponse
          {
              Id = formRaw.Id,
              IdCode = formRaw.IdCode,
              Title = formRaw.Title,
              ExpireDate = formRaw.ExpireDate,
              IsPublicForm = formRaw.IsPublicForm,
              State = formRaw.State,
              TCreate = formRaw.TCreate,
              TEdit = formRaw.TEdit,
              IsStrongRow = log == null,
              IsRead = log == null,
          };

            return await paginationData(pageNumber, pageSize, forms, sortFields, cancellationToken);

        }
  
        public async Task<RepeatedResult> GetIsRpeatedIdCode(Guid id, ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var result = await ValidateUniqueAsync(id, "IdCode", idCode, "شماره فرم", dbContext, cancellationToken);
            return result;

        }

        public async Task<IEnumerable<FormListResponse>> GetFormWithTitleOptionsForUser(
            IEnumerable<Guid> idUser,
            string title,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken
            )
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var results =await dbContext.Set<FormRawSys>()
             .Include(f => f.UserFund)
             .ThenInclude(fu => fu.User)
            .Where(
            FormsItem =>
                   (EF.Functions.Like(FormsItem.Title, $"%{title}%") &&
                     (FormsItem.IsPublicForm == true || FormsItem.UserFund.Any(itemUser => idUser.Contains(itemUser.IdUser)))
                     && ((FormsItem.ExpireDate > 0 && FormsItem.ExpireDate > nowUnixTimeSeconds) || FormsItem.ExpireDate == 0)

                   )
                     )
                 .ToArrayAsync();

            return results.Adapt<IEnumerable<FormListResponse>>();
        }

        public async Task<IEnumerable<FormListResponse>> GetFormWithIdCodeOptionsForUser(
        IEnumerable<Guid> idUser,
        ulong idCode,
        FormsInfoDbContext dbContext,
        CancellationToken cancellationToken
        )
        {
            long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            IEnumerable<FormRawSys> results;
            var resultFirst = await dbContext.Set<FormRawSys>()
            
         .Include(f => f.UserFund)
         .ThenInclude(fu => fu.User)

        .FirstOrDefaultAsync(item => item.IdCode == idCode);

            if (
                resultFirst != null &&
                ((await dbContext.FormUserSyss.AnyAsync(item => item.IdFormRaw == resultFirst.Id &&  idUser.Contains(item.IdUser) && item.IdUserRead == Guid.Empty)) == false
                &&
                ((resultFirst.ExpireDate > 0 && resultFirst.ExpireDate > nowUnixTimeSeconds) ||
                resultFirst.ExpireDate == 0)
                &&
                (resultFirst.IsPublicForm == true || resultFirst.UserFund.Any(itemUser => idUser.Contains(itemUser.IdUser))))
                )
                    {
                        results = [resultFirst];
                    }
                    else
                    {
                        results = null;
                    }

            return results.Adapt<IEnumerable<FormListResponse>>();
        }


        public async Task<IEnumerable<FormListResponse>> GetFormWithIdOptionsForUser(
        IEnumerable<Guid> idUser,
        Guid id,
        FormsInfoDbContext dbContext,
        CancellationToken cancellationToken
        )
        {
            //long nowUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //IEnumerable<FormRawSys> results;
            //var resultFirst = await dbContext.Set<FormRawSys>()

            IEnumerable<FormRawSys> results;
            var resultFirst = await dbContext.Set<FormRawSys>()
            .Include(f => f.UserFund)
            .ThenInclude(fu => fu.User)
                  .FirstOrDefaultAsync(item => item.Id == id);

            if (resultFirst == null)
                results= null;

            else if (resultFirst.IsPublicForm == true || resultFirst.UserFund.Any(uFund => idUser.Contains(uFund.Id)))
            {
                results = [resultFirst];
            }
            else
            {
                results = null;
            }

                return results.Adapt<IEnumerable<FormListResponse>>();
        }

    }
}
 