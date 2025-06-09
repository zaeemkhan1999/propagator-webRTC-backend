using Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

namespace Apsy.App.Propagator.Domain.Common.Inputs
{
    public class ExportConversationInput:BaseInputDef
    {
        public int UserID { get; set; }
        public string MessageJson { get; set; }
        public DateTime ExpirtyDate { get; set; }
    }
}
