using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Application.Common
{
    public class UserInfoDto : DtoDef
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string ImageAddress { get; set; }
        public string Cover { get; set; }
    }
}