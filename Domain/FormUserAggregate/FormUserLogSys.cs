using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORG.BasicInfo.Domain.FormUserAggregate
{
    public class FormUserLogSys
    {
        public Guid Id { get; private set; }
        public Guid IdFormRaw { get; private set; }
        public Guid IdFormUser { get; private set; }
        public string Description { get; private set; }
        public Guid IdUser { get; private set; }
        public Guid IdUserRead { get; private set; }
        public string Ip { get; set; }
        //public Guid IdUserAdmin { get; private set; }
        public StateAction StateAction { get; private set; }
        public long Timestamp { get; private set; }

        public FormUserLogSys(Guid idFormUser,
            Guid idFormRaw,
            string description,
            Guid idUser,

            Guid idUserRead,
            string ip,
            StateAction stateAction)
        {
            Id = Guid.NewGuid();
            IdFormUser = idFormUser;
            Description = description;
            IdUser = idUser;
            Ip = ip;
            IdUserRead = idUserRead;
            //IdUserAdmin = idUserAdmin;
            IdFormRaw = idFormRaw;
            StateAction = stateAction;
            Timestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

    }
}
