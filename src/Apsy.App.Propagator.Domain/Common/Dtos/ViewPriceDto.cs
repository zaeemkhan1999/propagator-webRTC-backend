namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ViewPriceDto : DtoDef
    {
        /// <summary>
        /// price for numberOf people per unit
        /// </summary>
        public int InitialPrice { get; set; }

        /// <summary>
        /// total view
        /// </summary>
        public int NumberOfPeoplePerUnit { get; set; }

    }
}