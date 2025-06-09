namespace Apsy.App.Propagator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AccountController(IConfiguration configuration,
        IAuthService authService)
    {
        _configuration = configuration;
        _authService = authService;
    }

    [HttpPost("emaillogin")]
    public async Task<IActionResult> EmailLoginAsync()
    {
        try
        {
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody)!;
            var apiKey = _configuration["ApiKey"];
            var authConfig = new AuthConfig { ApiKey = apiKey };
            var authToken = await _authService.EmailLogin(authConfig, Request.Query["email"], Request.Query["password"]);
            return Ok(authToken);
        }
        catch
        {
            return NotFound("User Not Found");
        }
    }
}
