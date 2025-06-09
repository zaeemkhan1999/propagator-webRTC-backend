namespace Apsy.App.Propagator.Domain.Entities
{
    public class ApplicationLogs:EntityDef
    {
        public string RequestName { get; set; }
        public string RequesetId { get; set; }
        public string RequestParameters { get; set; }
        public string ResponseParameters { get; set; }
    }
}
