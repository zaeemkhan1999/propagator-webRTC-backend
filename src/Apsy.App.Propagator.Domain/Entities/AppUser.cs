using Microsoft.AspNetCore.Identity;

namespace Apsy.App.Propagator.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public User User { get; set; }
        public int UserId { get; set; }

        //public string VerificationCode { get; set; }
        //public DateTime VerificationCodeExpireTime { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string VerificationTwoFactorCode { get; set; }

        public string RefreshToken { get; set; }
        public ICollection<UserLogin> UserLogins { get; set; }

    }
}
