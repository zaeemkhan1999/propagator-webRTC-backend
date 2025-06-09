namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserLogin : EntityDef
    {
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }

        public string UserAgent { get; set; }

        public string SigInIp { get; set; }
        public DateTime SigInTime { get; set; }

        public string SignOutIp { get; set; }
        public DateTime? SigOutTime { get; set; }
        public bool IsSuspicious { get; set; }
    }
}