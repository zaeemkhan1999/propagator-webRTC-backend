namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ProductsDto : DtoDef
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public int sellerId { get; set; }
        public int Stock { get; set; }

        public List<Reviews> Reviews { get; set; }
        public List<ProductImagesDto> Images { get; set; } = new List<ProductImagesDto>();
        public ProductReviewDto ProductReview { get; set; }
        public CurrentUserDto Seller { get; set; }
    }

    public class ProductImagesDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
    public class ProductReviewDto
    {
        public int AverageRating { get; set; }

        public int TotalReviews { get; set; }
    }
}
