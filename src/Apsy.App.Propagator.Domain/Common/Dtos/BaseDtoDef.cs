using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class BaseDtoDef<T> : DtoDef
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
