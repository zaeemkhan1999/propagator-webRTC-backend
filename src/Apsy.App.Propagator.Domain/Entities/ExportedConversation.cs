namespace Apsy.App.Propagator.Domain.Entities
{
    public class ExportedConversation : EntityDef
    {
        public int UserID { get; set; }
        public string MessageJson { get; set; }
        public DateTime ExpirtyDate { get; set; }

    }
}

