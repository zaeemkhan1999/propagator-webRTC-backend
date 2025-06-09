using Aps.CommonBack.Base.Extensions;
using Apsy.App.Propagator.Api.GraphQL.ObjectTypes;
using Apsy.App.Propagator.Api.RequestInterception.GraphQLConfigs;
using Hangfire;
using Hangfire.MySql;
using HotChocolate.Types.Pagination;
using Microsoft.OpenApi.Models;
using Propagator.Api.GraphQL.ObjectTypes;
using Propagator.Api.GraphQL.Subscriptions;
using System.Reflection;

namespace Apsy.App.Propagator.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration, string appPath)
    {
        services.AddCors(options => options.AddPolicy("AllowAllOrigins", builder =>
            builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()));

       
        services.AddSingleton<IAuthService, FirebaseAuthService>();
        services.AddSingleton<FirebaseAppCreator>();
        var projectName = configuration["ProjectName"];

        services.AddScoped(p => p.GetRequiredService<IDbContextFactory<DataContext>>().CreateDbContext());
        services.AddScoped(p => p.GetRequiredService<IDbContextFactory<DataReadContext>>().CreateDbContext());
        services.AddScoped(p => p.GetRequiredService<IDbContextFactory<DataWriteContext>>().CreateDbContext());

        string connectionString = configuration.GetConnectionString("DbConnection");
        string connectionStringRead = configuration.GetConnectionString("DbConnectionRead");
        services.AddPooledDbContextFactory<DataReadContext>(options => options
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionStringRead), options =>
            {
                options.MigrationsAssembly("Apsy.App.Propagator.Infrastructure");
                options.UseNetTopologySuite();

            })
            .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider() }))
                .EnableSensitiveDataLogging());


        services.AddPooledDbContextFactory<DataContext>(options => options
          .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
          {
              options.MigrationsAssembly("Apsy.App.Propagator.Infrastructure");
              options.UseNetTopologySuite();

          })
          .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider() }))
              .EnableSensitiveDataLogging());

        services.AddPooledDbContextFactory<DataWriteContext>(options => options
         .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
         {
             options.MigrationsAssembly("Apsy.App.Propagator.Infrastructure");
             options.UseNetTopologySuite();

         })
         .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider() }))
             .EnableSensitiveDataLogging());

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.MaxFailedAccessAttempts = 8;

            options.User.RequireUniqueEmail = false;
        })

        .AddDefaultTokenProviders()
        .AddTokenProvider<UsernameTokenProvider<AppUser>>("Username")
        .AddEntityFrameworkStores<DataContext>();


        var signingKey = new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

        services
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
           {
               o.SaveToken = true;
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true, // on production make it true
                   ValidateAudience = true, // on production make it true
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = configuration["JWT:Issuer"],
                   ValidAudience = configuration["JWT:Audience"],
                   IssuerSigningKey = signingKey,
                   ClockSkew = TimeSpan.Zero,
               };
               o.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       {
                           context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                       }
                       return Task.CompletedTask;
                   }
               };
           });

        services.AddScoped<JwtUtilities>();

        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        services.AddHangfire(config =>
        {
            config.UseStorage(new MySqlStorage(
                connectionString,
                new MySqlStorageOptions
                {
                    TablesPrefix = "Hangfire" // Optional: Add a prefix to Hangfire tables
                }));
        });
        var options = new BackgroundJobServerOptions
        {
            ServerName = "Specterman",
        };
        services.AddHttpContextAccessor();
        services.AddHangfireServer();
        services.AddControllersWithViews();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
        });



        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AddArticleCommentEventHandler).Assembly);
        });

        services.AddHttpClient();
        //  services.AddScoped<CompressionApiClient>();


        #region RegisterServices


        var path = $"{appPath}/assets/persisted-queries";
        var graphql = services
            .AddGraphQLServer()
            .AddQueryType<QueryType>()
              .AddType<UploadType>()
            .AddMutationType<MutationType>()

            .AddSubscriptionType<SubscriptionType>()
            .AddInMemorySubscriptions()
            .AddHttpRequestInterceptor<CustomHttpRequestInterceptor>()
            .AddSpatialTypes()
            .AddFiltering()
            .AddConvention<IFilterConvention, FilterConventionExtensionForInvariantContainsStrings>()
            .AddSorting()
            .AddProjections()
            .SetPagingOptions(new PagingOptions
            {
                InferCollectionSegmentNameFromField = false,
                InferConnectionNameFromField = false,
            })
            .UsePersistedQueryPipeline()
            .AddReadOnlyFileSystemQueryStorage(path)
            .AddAuthorization();

        var extendMutations = ReflectionExtensions.LoadTypesFromAssemblies(typeof(Mutation).Assembly, t => t.IsClass)
            .Where(x => x.Namespace == typeof(Mutation).Namespace)
            .Where(x => x.GetCustomAttribute<ExtendObjectTypeAttribute>() != null);

        foreach (var exm in extendMutations)
        {
            graphql.AddTypeExtension(exm);
        }

        var extendQueries = ReflectionExtensions.LoadTypesFromAssemblies(typeof(Query).Assembly, t => t.IsClass)
            .Where(x => x.Namespace == typeof(Query).Namespace)
            .Where(x => x.GetCustomAttribute<ExtendObjectTypeAttribute>() != null);

        foreach (var exq in extendQueries)
        {
            graphql.AddTypeExtension(exq);
        }

        var extendSubscriptions = ReflectionExtensions.LoadTypesFromAssemblies(typeof(Subscription).Assembly, t => t.IsClass)
            .Where(x => x.Namespace == typeof(Subscription).Namespace)
            .Where(x => x.GetCustomAttribute<ExtendObjectTypeAttribute>() != null);

        foreach (var exs in extendSubscriptions)
        {
            graphql.AddTypeExtension(exs);
        }

        MethodInfo method = typeof(RequestExecutorBuilderExtensions).GetMethod(nameof(RequestExecutorBuilderExtensions.RegisterService));
        //foreach (var @interface in serviceTypes.Where(x => x.IsInterface && !x.CustomAttributes.Any(d => d.AttributeType == typeof(NonInjectabelAttribute))))
        //{
        //    MethodInfo generic = method.MakeGenericMethod(@interface);
        //    generic.Invoke(@interface, new object[] { graphql, ServiceKind.Default });
        //}
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        #endregion
    }


}