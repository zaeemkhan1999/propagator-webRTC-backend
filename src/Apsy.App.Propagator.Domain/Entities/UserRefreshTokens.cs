using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserRefreshTokens : EntityDef
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string RefreshToken { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
