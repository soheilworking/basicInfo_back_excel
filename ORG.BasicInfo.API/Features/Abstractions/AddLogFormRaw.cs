using ORG.BasicInfo.Data;
using ORG.BasicInfo.Domain.FormAggregate;

namespace ORG.BasicInfo.API.Features.Abstractions
{
    public class AddLogFormRaw
    {
        private readonly FormRawLogSys log;
        private readonly FormsInfoDbContext _context;
        public Guid IdFormUser { get; private set; }
        public string Description { get; private set; }
        public Guid IdUser { get; private set; }
        public Guid IdUserAdmin { get; private set; }
        public long Timestamp { get; private set; }
        public AddLogFormRaw(
            Guid idFormRaw,
            string description,
            Guid idUser,
            Guid idUserAdmin,
            string ip,
            FormsInfoDbContext context
            )
        {
            log = new FormRawLogSys(idUser,  idFormRaw,ip, description);
            _context = context;
        }
        public async Task SaveChange()
        {
            _context.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
