namespace Apsy.App.Propagator.Domain.Entities;

    public class AdminTodayLimitation : UserKindDef<User>
    {
        public int SuspendedCount { get; set; }
        public int BansCount { get; set; }
        public int StrikeCount { get; set; }
    }
