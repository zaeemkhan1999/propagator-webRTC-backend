namespace Apsy.App.Propagator.Domain.Common.Inputs
{
    public class ReviewInput: InputDef
    {
        public int ProductId { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public int UserId { get; set; }
    }
}

