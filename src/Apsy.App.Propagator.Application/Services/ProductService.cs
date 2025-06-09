using Apsy.App.Propagator.Domain.Common.Inputs;
using Apsy.App.Propagator.Domain.Entities;
using Apsy.App.Propagator.Infrastructure.Repositories.Contracts;

namespace Apsy.App.Propagator.Application.Services
{
    public class ProductService : ServiceBase<Product, ProductInput>, IProductService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public ProductService(
            IUserRepository userRepository,
            IFileUploadService fileUploadService,
            IProductRepository repository,
            IHttpContextAccessor httpContextAccessor
        ) : base(repository)
        {
            _userRepository = userRepository;
            _fileUploadService = fileUploadService;
            _productRepository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Product> CreateProductAsync(ProductInput input)
        {
            var CurrentUser = GetCurrentUser();
            if (input.Images.Count < 3 || input.Images.Count > 10)
                throw new ArgumentException("Images count must be between 3 and 10.");

            var uploadedImages = new List<ProductImages>();

            foreach (var imageFile in input.Images)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var postInput = new ProductInput { Images = new List<IFormFile> { imageFile } };
                    var fileUploadDto = _fileUploadService.SavefileinlocalfolderProduct(postInput);
                    var imageUrl = await _fileUploadService.FileUploaderAsync(fileUploadDto);

                    uploadedImages.Add(new ProductImages { ImageUrl = imageUrl });
                }
            }
            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Price = input.Price,
                Currency = input.Currency,
                CreatedDate = DateTime.Now,
                Stock = input.Stock,
                SellerId = input.SellerId

            };
            var productId= await _productRepository.AddAsync(product);
            foreach (var item in uploadedImages)
            {
                var productImage = new ProductImages
                {
                    ImageUrl = item.ImageUrl,
                    ProductId = productId.Id
                };
                await _productRepository.ProductImagesAsync(productImage);

            }

            return product;
        }

        public async Task<Product> UpdateProductAsync(UpdateProductInput input)
        {

            var check = await _productRepository.GetProductDetails(input.Id);

            if (check != null)
            {

                //Id = input.Id,
                check.Stock = input.Stock != 0 ? input.Stock : check.Stock;
                check.Name = input.Name ?? check.Name;
                check.Description = input.Description ?? check.Description;
                check.Price = input.Price != 0 ? input.Price : check.Price;
                check.Currency = input.Currency ?? check.Currency;
                check.LastModifiedDate = DateTime.Now;


                if (input?.ProductImageIds?.Count > 0)
                {
                    await _productRepository.DeleteProductImages(input.ProductImageIds);
                }

                if (input?.Images != null)
                {
                    var uploadedImages = new List<ProductImages>(); 
                    foreach (var imageFile in input.Images)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var postInput = new ProductInput { Images = new List<IFormFile> { imageFile } };
                            var fileUploadDto = _fileUploadService.SavefileinlocalfolderProduct(postInput);
                            var imageUrl = await _fileUploadService.FileUploaderAsync(fileUploadDto);

                            uploadedImages.Add(new ProductImages { ImageUrl = imageUrl });
                        }
                    }
                    foreach (var item in uploadedImages)
                    {
                        var productImage = new ProductImages
                        {
                            ImageUrl = item.ImageUrl,
                            ProductId = input.Id
                        };

                        await _productRepository.ProductImagesAsync(productImage);

                    }
                   
                }

                var updateDetails = await _productRepository.UpdateProductAsync(check);
                return updateDetails;
            }

            return null;

        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var result = await _productRepository.GetProductDetails(productId);
            //var images = await _productRepository.GetProductImages(productId);

            // Initialize the Images list if it is null
            if (result.Images == null)
            {
                result.Images = new List<ProductImages>();
            }

            // Append the images list to product.Images
            //result.Images.AddRange(images);
            return result;
        }
        public async Task<Reviews> AddProductReview(ReviewInput input)
        {
            var review = new Reviews
            {
                ProductId = input.ProductId,
                Description = input.Description,
                Rating = input.Rating,
                UserId = input.UserId,
                CreatedDate = DateTime.Now
            };

            var result = await _productRepository.AddReviewAsync(review);

            var user = await _userRepository.GetByIdAsync(input.UserId);

            result.User = new User
            {
                Id = user.Id,
                Username = user.Username,
                ImageAddress = user.ImageAddress

            };

            return result;
        }
        private User GetCurrentUser(string token = null)
        {
            var User = _httpContextAccessor.HttpContext.User;
            if (!User.Identity.IsAuthenticated)
                return null;
            var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
            var user = JsonConvert.DeserializeObject<User>(userString);
            return user;
        }
    }
}
