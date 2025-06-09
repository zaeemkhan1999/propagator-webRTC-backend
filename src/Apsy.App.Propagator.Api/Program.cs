using Apsy.App.Propagator.Api.Extensions;
using Apsy.App.Propagator.Application.Data;
using Apsy.App.Propagator.Domain.ViewModel;
using Hangfire;
using LiteDB;
using Microsoft.AspNetCore.Http.Features;
using Propagator.Api.Extensions;
using StackExchange.Redis;
using Stripe;
using Twilio;

var builder = WebApplication.CreateBuilder(args);

var ctxenv = builder.Environment.EnvironmentName;
var appPath = builder.Environment.ContentRootPath;

builder.Configuration.AddJsonFile($"{appPath}/appsettings.json", optional: false, reloadOnChange: true);

var env = Environment.GetEnvironmentVariable("env");
if (new[] { "dev", "qa", "stg", "prd" }.Contains(env))
{
    builder.Configuration.AddJsonFile($"{appPath}/appsettings.{env}.json", optional: false, reloadOnChange: true);
}
builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;
builder.Services.AddServices(configuration, appPath);
builder.Services.AddMyAuthorization();
builder.Services.RegisterServices();
builder.Services.RegisterRepository();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<TwilioVerifySettingsVM>(configuration.GetSection("Twilio"));
// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // 1GB
});

// Configure IIS
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 1073741824;
    options.MaxRequestBodyBufferSize = 1073741824;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var redisHost = configuration["Redis:Host"]; 
    var redisPort = configuration["Redis:Port"]; 

    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { $"{redisHost}:{redisPort}" }, 
        SyncTimeout = 5000,
        AsyncTimeout = 5000
    };

    return ConnectionMultiplexer.Connect(configurationOptions);
});

var app = builder.Build();

//if (new[] { "dev", "qa", "stg" }.Contains(env) || !app.Environment.IsProduction())
{
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}
app.UseRouting();
app.UseHangfireDashboard();
app.UseCors("AllowAllOrigins");
app.Use(async (context, next) =>
    {
    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 1073741824; // 100 MB
    await next.Invoke();
});
app.MapGraphQL("/api/graphql");

app.UseStaticFiles();

app.UseWebSockets();

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

StripeConfiguration.ApiKey = configuration.GetValue<string>("Stripe:StripeConfigSK");
var accountSid = configuration["Twilio:AccountSID"];
var authToken = configuration["Twilio:AuthToken"];
TwilioClient.Init(accountSid, authToken);
DataInitializer.Initialize(app.Services);

app.Run();
