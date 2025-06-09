using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;

namespace Apsy.App.Propagator.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product, DataReadContext>, IProductRepository
    {
        private readonly DataReadContext _context;
        public ProductRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
        {
            _context = dbContextFactory.CreateDbContext();
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Product.Add(product);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public async Task<Reviews> AddReviewAsync(Reviews review)
        {
            _context.Reviews.Add(review);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return review;
        }
        public async Task<ProductImages> ProductImagesAsync(ProductImages productImages)
        {
            _context.ProductImages.Add(productImages);
            await _context.SaveChangesAsync();
            return productImages;
        }

        public async Task<Product> GetProductDetails(int productId)
        {
            var product = await _context.Product
               .Include(p => p.Review)
               .Include(p => p.Images)
               .Include(p => p.Seller)
               .FirstOrDefaultAsync(p => p.Id == productId) ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");
            return product;
        }

        public async Task<List<ProductImages>> GetProductImages(int productId)
        {
            var images = await _context.ProductImages
                               .Where(p => p.ProductId == productId)
                               .OrderByDescending(p => p.CreatedDate)
                               .ToListAsync();
            return images;
        }
        public async Task<Product> UpdateProductAsync(Product product)
        {
            // Retrieve the product from the database
            // var result = await _context.Product.FindAsync(product.Id);

            // if (result == null)
            // {
            //     throw new KeyNotFoundException($"Product with ID {result.Id} not found.");
            // }


            _context.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteProductImages(List<int> ProductImageIdes)
        {
            var products = _context.ProductImages.Where(x => ProductImageIdes.Contains(x.Id));
            _context.ProductImages.RemoveRange(products);
            await _context.SaveChangesAsync();

                }
    }
}
