namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class GroupRequestDto:DtoDef
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int GroupAdminId { get; set; }
        public string Status { get; set; }

        public User user {get; set;}
    }
}
