using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORG.BasicInfo.Domain.FormAggregate
{
    public class FormRawLogSys
    {
        public Guid Id { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdForm { get; set; }
        public long Timestamp { get; set; }
        public string Ip { get; set; }
        public string Description { get; set; }
        public FormRawLogSys(
            Guid idUser,
            Guid idForm,
            string ip,
            string description)
        {
            Id = Guid.NewGuid();
            IdUser = idUser;
            IdForm = idForm;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Description = description;
            Ip = ip;

        }
    }
}
