using Apsy.App.Propagator.Domain.Common.Inputs;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Propagator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService productService, IConfiguration configuration)
        {
            _productService = productService;
            _configuration = configuration;
        }
        [HttpPost("CreateAsync")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] ProductInput input)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = ValidateToken(token);
                if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
                    return Unauthorized(new { Message = "Authentication failed." });
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var product = await _productService.CreateProductAsync(input);
                return Ok(new
                {
                    Message = "Product created successfully",
                    // Product = product.Id,
                    // product.Name,
                    // product.Price,
                    // product.Description,
                    // product.Currency,
                    // product.Review,
                    // product.Stock,
                    // product.SellerId,
                    // product.CreatedDate,
                    // product.IsDeleted,
                    // product.LastModifiedDate,
                    // product.Images
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }
        [HttpPost("UpdateProduct")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductInput input)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = ValidateToken(token);
                if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
                    return Unauthorized(new { Message = "Authentication failed." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.UpdateProductAsync(input);

                return Ok(new { Message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        [HttpGet("GetProductDetails")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = ValidateToken(token);
                if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
                    return Unauthorized(new { Message = "Authentication failed." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.GetProductAsync(productId);

                return Ok(new { Message = "Product Detail successfully retrieved", Product = product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        [HttpPost("AddProductReview")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProductReview([FromForm] ReviewInput input)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = ValidateToken(token);
                if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
                    return Unauthorized(new { Message = "Authentication failed." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.AddProductReview(input);

                return Ok(new { Message = "Product Review successfully added", Product = product });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
