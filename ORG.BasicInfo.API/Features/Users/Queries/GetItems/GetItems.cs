using Mapster;
using Microsoft.EntityFrameworkCore;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.API.Features.Users.Queries.TResponse;
using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.UserAggregate;
using System.Linq;

namespace ORG.BasicInfo.API.Features.Users.Queries
{
    public class GetItems : IGetItems<UserListResponse, UserInfoResponse, User, FormsInfoDbContext>
    {

        public override async Task<ResponseQuery<UserListResponse>> GetInfoAll(int pageNumber,
            int pageSize,
            IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Users = dbContext.Users
            .AsNoTracking();
           
            return await paginationData(pageNumber, pageSize, Users, sortFields, cancellationToken); 

        }
        public async Task<ResponseQuery<UserListResponse>> GetInfoAllOptionsFundName(string fundName,IEnumerable<Guid> idsSelected,FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            IEnumerable<User> users=null;
    
                users = await dbContext.Users
                .AsNoTracking()
                 .Where(
                    UsersItem => (EF.Functions.Like(UsersItem.FundName, $"%{fundName}%")) || 
                    (idsSelected != null && idsSelected.Contains(UsersItem.Id)) )
                 .Take(30)
                .ToArrayAsync();
            
            var listUserss = users.Adapt<IEnumerable<UserListResponse>>();
            return new ResponseQuery<UserListResponse> { Items = listUserss, Counts = (ulong)listUserss.Count() };
        }
        public async Task<ResponseQuery<UserListResponse>> GetInfoAllOptionsFundCode(ulong fundCode, IEnumerable<Guid> idsSelected, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            //User users = null;
            IEnumerable<User> users = null;

            users = await dbContext.Users
            .AsNoTracking()
             .Where(
                UsersItem => UsersItem.FundCode==fundCode ||
                (idsSelected != null && idsSelected.Contains(UsersItem.Id)))
             .Take(30)
            .ToArrayAsync();
            
            var listUserss = users.Adapt<IEnumerable<UserListResponse>>();
            return new ResponseQuery<UserListResponse> { Items = listUserss, Counts = (ulong)listUserss.Count() };
        }
        public async Task<ResponseQuery<UserListResponse>> GetInfoAllOptionsFundNameForUser(string fundName,IEnumerable<Guid> idUsers, IEnumerable<Guid> idsSelected, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = null;

            users = await dbContext.Users
            .AsNoTracking()
             .Where(
                UsersItem => 
                ((EF.Functions.Like(UsersItem.FundName, $"%{fundName}%")) ||
                (idsSelected != null && idsSelected.Contains(UsersItem.Id)))&&
                idUsers.Contains(UsersItem.Id))
             .Take(30)
            .ToArrayAsync();

            var listUserss = users.Adapt<IEnumerable<UserListResponse>>();
            return new ResponseQuery<UserListResponse> { Items = listUserss, Counts = (ulong)listUserss.Count() };
        }
        public async Task<ResponseQuery<UserListResponse>> GetInfoAllOptionsFundCodeForUser(ulong fundCode, IEnumerable<Guid> idUsers, IEnumerable<Guid> idsSelected, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            //User users = null;
            IEnumerable<User> users = null;

            users = await dbContext.Users
            .AsNoTracking()
             .Where(
               UsersItem => (UsersItem.FundCode == fundCode ||
                (idsSelected != null && idsSelected.Contains(UsersItem.Id))) &&
                idUsers.Contains(UsersItem.Id)
                )
             .Take(30)
            .ToArrayAsync();

            var listUserss = users.Adapt<IEnumerable<UserListResponse>>();
            return new ResponseQuery<UserListResponse> { Items = listUserss, Counts = (ulong)listUserss.Count() };
        }

        public override async Task<UserInfoResponse> GetInfoWithId(Guid id, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var users = await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == id);
           
            var idEnc=await PermissionFund.GetEncryptDataWithThisKey(id.ToString());
            var pFunds = (await dbContext.PermissionFunds
                .AsNoTracking()
                .Where(item => item.IdUser == idEnc).ToListAsync());
                
            var pFundsIds= await Task.WhenAll(pFunds.Select(async item=>await item.GetIdFund()));
            var result= users.Adapt<UserInfoResponse>();
            result.UserRole =(await users.GetUserRole()).ToString();
            result.PermissionFunds = pFundsIds;
            return result;
        }

        public override async Task<UserListResponse> GetInfoWithIdCode(ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Users = await dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(item => item.FundCode == idCode);
    
              

            return Users.Adapt<UserListResponse>();
        }
  
        public  async Task<UserListResponse> GetInfoWithMobile(ulong mobile, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var Users = await dbContext.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(item => item.Mobile == mobile);



            return  Users.Adapt<UserListResponse>();
        }

        public override async Task<ResponseQuery<UserListResponse>> SearchWithNameLike(string name, 
            int pageNumber, 
            int pageSize,
             IEnumerable<string[]> sortFields,
            FormsInfoDbContext dbContext, 
            CancellationToken cancellationToken)
        {

            var Users = dbContext.Users
                  .AsNoTracking()
                  .Where(UsersItem => EF.Functions.Like(UsersItem.Name, $"%{name}%")|| EF.Functions.Like(UsersItem.LastName, $"%{name}%"));
             
            return await paginationData(pageNumber, pageSize, Users, sortFields, cancellationToken);

        }
        public async Task<ResponseQuery<UserListResponse>> SearchWithFundNameLike(string name,
          int pageNumber,
          int pageSize,
           IEnumerable<string[]> sortFields,
          FormsInfoDbContext dbContext,
          CancellationToken cancellationToken)
        {

            var Users = dbContext.Users
                  .AsNoTracking()
                  .Where(UsersItem => EF.Functions.Like(UsersItem.FundName, $"%{name}%"));

            return await paginationData(pageNumber, pageSize, Users, sortFields, cancellationToken);

        }
        public async Task<RepeatedResult> GetIsRpeatedIdCode(Guid id, ulong idCode, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var result = await ValidateUniqueAsync(id, "FundCode", idCode, "شماره صندوق", dbContext, cancellationToken);
            return result;

        }
        public async Task<RepeatedResult> GetIsRpeatedMobile(Guid id, ulong mobile, FormsInfoDbContext dbContext, CancellationToken cancellationToken)
        {
            var result = await ValidateUniqueAsync(id, "Mobile", mobile, "شماره همراه", dbContext, cancellationToken);
            return result;

        }
    

    }
}
 