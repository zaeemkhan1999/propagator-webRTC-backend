namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

    public class GroupRequestInput: InputDef
    {
        public int GroupId { get; set; }
        public int GroupAdminId { get; set; }
    }