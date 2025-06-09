using Apsy.App.Propagator.Infrastructure.ReadRepositories.Contracts;

namespace Apsy.App.Propagator.Infrastructure.ReadRepositories
{
    public class ProductReadRepository : Repository<Product, DataReadContext>, IProductReadRepository
    {
        private readonly DataReadContext _context;
        public ProductReadRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
        {
            _context = dbContextFactory.CreateDbContext();
        }

        public async Task<List<ProductsDto>> GetProductDetails(int id)
        {
            var products = await _context.Product
              .Include(p => p.Images)
              .Include(p => p.Review).ThenInclude(x=>x.User)
              .Include(p => p.Seller)
             .OrderByDescending(x => x.CreatedDate).Where(x=>x.Id==id)
          .ToListAsync();

            return products.Select(p => new ProductsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = Math.Round(p.Price, 2),
                Currency = p.Currency,
                Stock = p.Stock,
                Images = p.Images?.Select(img => new ProductImagesDto { Id=img.Id,   Url = img.ImageUrl }).ToList(),
                ProductReview = p.Review != null && p.Review.Any() ? new ProductReviewDto
                {
                    TotalReviews = p.Review.Count,
                    AverageRating = (int)Math.Round(p.Review.Average(r => r.Rating))
                     
                } : new ProductReviewDto { TotalReviews = 0, AverageRating = 0 },
                sellerId = p.SellerId,
                Reviews=p.Review,
                Seller = p.Seller != null ? new CurrentUserDto
                {
                    DisplayName = p.Seller.DisplayName,
                    Username = p.Seller.Username,
                    Id = p.Seller.Id
                } : null
            }).ToList();
        }

        public async Task<List<ProductsDto>> GetProductsAsync()
        {
            var products = await _context.Product
                .Include(p => p.Images)
                .Include(p => p.Review)
                .Include(p=>p.Seller)
               .OrderByDescending(x=>x.CreatedDate)
            .ToListAsync();

            return products.Select(p => new ProductsDto
            {
                Id=p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = Math.Round(p.Price, 2),
                Currency = p.Currency,
                Stock = p.Stock,
                Images = p.Images?.Select(img => new ProductImagesDto { Url = img.ImageUrl }).ToList() ,
                ProductReview = p.Review != null && p.Review.Any() ? new ProductReviewDto
                {
                    TotalReviews = p.Review.Count,
                    AverageRating = (int)Math.Round(p.Review.Average(r => r.Rating)) 
                } : new ProductReviewDto { TotalReviews = 0, AverageRating = 0 },
                sellerId=p.SellerId,
                Seller =p.Seller !=null ? new CurrentUserDto
                {
                    DisplayName=p.Seller.DisplayName,
                    Username=p.Seller.Username,
                    Id =p.Seller.Id
                }:null
            }).ToList();
        }
    }
}
