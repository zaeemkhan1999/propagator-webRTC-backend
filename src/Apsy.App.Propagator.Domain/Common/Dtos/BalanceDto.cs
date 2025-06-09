namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class BalanceDto
    {
        public string Object { get; set; }

        public List<BalanceAmountDto> Available { get; set; }

        public List<BalanceAmountDto> ConnectReserved { get; set; }

        public List<BalanceAmountDto> Pending { get; set; }
    }
}
