
using ORG.BasicInfo.Domain.Abstractions;

namespace ORG.BasicInfo.API.Features.Abstractions
{
    public abstract class AResponse
    {
        //public Guid Id{ get; set; }

        public long TCreate { get; set; }
        public long TEdit{ get; set; }

        public EntityState State{ get; set; }
    }



}
