namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class AdsInput : BaseInputDef
{

    public bool? VisitType { get; set; }

    public string TargetLocation { get; set; }

    public int? TargetStartAge { get; set; }

    public int? TargetEndAge { get; set; }

    public bool? ManualStatus { get; set; }

    public int? NumberOfPeopleCanSee { get; set; }

    public int? TotalViewed { get; set; }

    public int? TicketNuber { get; set; }

    public int? AdsId { get; set; }

    public int? PaymentId { get; set; }


}
