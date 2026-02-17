using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormUserAggregate;

namespace ORG.BasicInfo.API.Features.Abstractions
{
    public class AddLogFormUser
    {
        private readonly FormUserLogSys log;
        private readonly FormsInfoDbContext _context;

        public AddLogFormUser(
            Guid idFormUser,
            Guid idFormRaw,
            string description,
            Guid idUser,
            Guid idUserRead,
            string ip,
            StateAction stateAction,
            FormsInfoDbContext context
            )
        {
            log = new FormUserLogSys(idFormUser,
                idFormRaw,
                description,
                idUser,
                idUserRead,
                ip,
                stateAction);
            _context = context;
        }
        public async Task SaveChange()
        {
            _context.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
