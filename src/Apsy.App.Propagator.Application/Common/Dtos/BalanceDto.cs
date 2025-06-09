namespace Apsy.App.Propagator.Application.Common
{
    public class BalanceDto
    {
        public string Object { get; set; }

        public List<BalanceAmountDto> Available { get; set; }

        public List<BalanceAmountDto> ConnectReserved { get; set; }

        public List<BalanceAmountDto> Pending { get; set; }
    }
}
