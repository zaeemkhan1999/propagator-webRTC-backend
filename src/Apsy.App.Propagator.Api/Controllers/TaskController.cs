using Apsy.App.Propagator.Domain.Common.Dtos.Dtos;

namespace Propagator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IPostService _postService;


        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSettings;

        public TaskController(IConfiguration configuration, IPostService postService,
            IAuthService authService, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _authService = authService;
            _postService = postService;

            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                FloatParseHandling = FloatParseHandling.Decimal
            };
        }
          [HttpPost]
        public IActionResult TaskCallBack(TaskResquestModel resquestModel,int PostId)
        {
            try
            {
                var res = _postService.UpdatePostsCompressedResponse(resquestModel, PostId);
             return Ok("Success");
            }
            catch (Exception ex)
            {

                return Ok(ex.Message);
            }
        }
    }
}
