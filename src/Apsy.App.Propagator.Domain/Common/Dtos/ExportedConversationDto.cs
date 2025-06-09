namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ExportedConversationDto
    {
        public int UserID { get; set; }
        public List<Message> MessageJson { get; set; }
        public DateTime ExpirtyDate { get; set; }
    }
}
