using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Application.Common
{
    public class AdminInfoDto : DtoDef
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
