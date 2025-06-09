using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class UserSusciopiousDto : DtoDef
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string DisplayName { get; set; }

        public string UserAgent { get; set; }

        public string SigInIp { get; set; }
        public DateTime SigInTime { get; set; }

        public string SignOutIp { get; set; }
        public DateTime? SigOutTime { get; set; }
        public bool IsSuspicious { get; set; }
        public string ImageAddress { get; set; }
        public string Cover { get; set; }
        public string UserName { get; set; }
    }
}