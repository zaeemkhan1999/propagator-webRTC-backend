namespace Apsy.App.Propagator.Application.Common
{
    public class CompressionApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerSettings _jsonSettings;

        public CompressionApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                FloatParseHandling = FloatParseHandling.Decimal
            };

        }

        public async Task<HttpResponseMessage> ConvertAsync(Post request)
        {
            var client = _httpClientFactory.CreateClient();
            var ApiKey = _configuration["Conversion:ApiKey"];

            string callbackUrl = _configuration["CallbackBaseUrl"] + "api/Task?PostId=" + request.Id;
            var payload = new ConversionRequest
            {
                Id = request.Id,
                PostItems = request.PostItemsString,
                CallbackUrl = callbackUrl
            };

            var jsonContent = JsonConvert.SerializeObject(payload, _jsonSettings);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://compression.api.specterman.io/convert")
            {
                Content = content
            };

            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            httpRequest.Headers.Add("x-api-key", ApiKey);

            return await client.SendAsync(httpRequest);
        }

        public async Task<HttpResponseMessage> FollowupTaskDetails(int TaskId)
        {
            var client = _httpClientFactory.CreateClient();
            var ApiKey = _configuration["Conversion:ApiKey"];

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://compression.api.specterman.io/followup/task/" + TaskId);


            httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            httpRequest.Headers.Add("x-api-key", ApiKey);

            return await client.SendAsync(httpRequest);
        }

        public async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString, _jsonSettings);
        }
    }

    public class ConversionRequest
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("PostItems")]
        public string PostItems { get; set; }

        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
    }

}
