namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class BuyTokenDto
    {
        public BuyTokenDto(string clientSecret, double amount)
        {
            ClientSecret = clientSecret;
            Amount = amount;
        }

        public string ClientSecret { get; private set; }
        public double Amount { get; private set; }
    }
}
