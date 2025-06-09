using Apsy.App.Propagator.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Apsy.App.Propagator.Application.Data;

public class DataInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        Task.Run(async () =>
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var isTryString = configuration["IsTry"];
            var isTry = string.Equals(bool.TrueString, isTryString, StringComparison.CurrentCultureIgnoreCase);
            if (isTry)
            {
                var dataContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var dataContext = await dataContextFactory.CreateDbContextAsync();

                await dataContext.Database.MigrateAsync();

                var transaction = await dataContext.Database.BeginTransactionAsync();

                try
                {
                    await SeedSettingsDataAsync(dataContext);
                    await SeedAdminsDataAsync(dataContext, configuration, userService);
                    await SeedOwnerDataAsync(dataContext, configuration, userService);
                    await SeedConversationGroupDataAsync(dataContext);
                    await SeedSecurityQuestionDataAsync(dataContext, configuration);
                    await SeedSubscriptionPlans(dataContext, configuration);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    //var logger = builder.Services.GetRequiredService<ILogger<Startup>>();
                    //logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        });
    }

    private static async Task SeedAdminsDataAsync(DbContext dataContext, IConfiguration configuration, IUserService userService)
    {
        var admin = configuration.GetSection("SuperAdmin").Get<SuperAdminDto>();

        if (admin == null)
            return;

        var entityUser = await dataContext.Set<User>().Include(c => c.AppUser).FirstOrDefaultAsync(c => c.Email == admin.Email);
        if (entityUser != null && entityUser.UserTypes != UserTypes.SuperAdmin)
        {
            SetUserData(entityUser);
            await dataContext.SaveChangesAsync();
        }
        else if (entityUser == null)
        {
            var signUpInput = new SignUpInput()
            {
                Email = admin.Email,
                Password = admin.Password,
                //DisplayName = "superAdmin",
                //DateOfBirth = DateTime.UtcNow.AddYears(-24),
                //EnableTwoFactorAuthentication = false,
                Gender = Gender.Male,
                Username = admin.Username
            };
            var user = await userService.InsertUser(signUpInput);
            entityUser = await dataContext
                .Set<User>()
                .Include(x => x.AppUser)
                .SingleOrDefaultAsync(x => x.Id == user.Result.UserId);
            SetUserData(entityUser);

            await dataContext.SaveChangesAsync();
        }
    }

    private static async Task SeedOwnerDataAsync(DbContext dataContext, IConfiguration configuration, IUserService userService)
    {
        var owner = configuration.GetSection("Owner").Get<SuperAdminDto>();

        if (owner == null)
            return;

        var entityUser = await dataContext.Set<User>().Include(c => c.AppUser).FirstOrDefaultAsync(c => c.Email == owner.Email);
        if (entityUser != null && entityUser.UserTypes != UserTypes.SuperAdmin)
        {
            SetUserData(entityUser);
            await dataContext.SaveChangesAsync();
        }
        else if (entityUser == null)
        {
            var signUpInput = new SignUpInput()
            {
                Email = owner.Email,
                Password = owner.Password,
                DisplayName = "superAdmin",
                //DateOfBirth = DateTime.UtcNow.AddYears(-24),
                //EnableTwoFactorAuthentication = false,
                Gender = Gender.Male,
                Username = owner.Username
            };
            var user = await userService.InsertUser(signUpInput);
            entityUser = await dataContext
                .Set<User>()
                .Include(x => x.AppUser)
                .SingleOrDefaultAsync(x => x.Id == user.Result.UserId);
            SetUserData(entityUser);

            await dataContext.SaveChangesAsync();
        }
    }


    private static void SetUserData(User entityUser)
    {
        entityUser.UserTypes = UserTypes.SuperAdmin;
        entityUser.IsActive = true;
        entityUser.IsDeletedAccount = false;
        entityUser.AppUser.EmailConfirmed = true;
    }

    //private static void SeedData()
    //{
    //    User Admin = _configuration.GetSection("SuperAdmin").Get<User>();
    //    if (Admin == null)
    //        return;

    //    var entityUser = dataContext.Set<User>();
    //    if (entityUser.Any(x => x.ExternalId == Admin.ExternalId))
    //    {
    //        Admin = entityUser.First(x => x.ExternalId == Admin.ExternalId);
    //        Admin.UserTypes = UserTypes.Admin;
    //        Admin.IsActive = true;
    //        Admin.IsDeletedAccount = false;
    //        dataContext.SaveChanges();
    //    }
    //    else
    //    {
    //        Admin.UserTypes = UserTypes.SuperAdmin;
    //        Admin.IsActive = true;
    //        Admin.IsDeletedAccount = false;
    //        Admin.Username = string.Empty;
    //        dataContext.Add(Admin);
    //        dataContext.SaveChanges();
    //    }
    //}

    private static async Task SeedSettingsDataAsync(DbContext dataContext)
    {
        var settings = dataContext.Set<Settings>();
        if (!settings.Any())
        {
            var setting = new Settings();
            dataContext.Add(setting);
            await dataContext.SaveChangesAsync();
        }
    }

    private static async Task SeedConversationGroupDataAsync(DbContext dataContext)
    {
        const string groupName = "Propagator Council";
        const string groupDescription = "Propagator Council";
        var superAdmin = await dataContext.Set<User>().FirstOrDefaultAsync(x => x.UserTypes == UserTypes.SuperAdmin);
        var conversation = await dataContext.Set<Conversation>().FirstOrDefaultAsync(x => x.GroupName == groupName);

        if (conversation == null && superAdmin != null)
        {
            var newConversation = new Conversation()
            {
                IsGroup = true,
                GroupName = groupName,
                GroupDescription = groupDescription,
                IsPrivate = true,
                GroupLink = "GroupLink",
                GroupImgageUrl = "GroupImgageUrl",
                AdminId = superAdmin?.Id,
                UserGroups = new List<UserMessageGroup>()
                {
                    new(){
                        IsAdmin =true,
                        UserId  = superAdmin.Id,
                        UnreadCount = 0,
                        LastModifiedDate =DateTime.UtcNow,
                        //MembershipExpirationTime = DateTime.UtcNow.AddYears(100)
                    }
                }
            };

            dataContext.Add(newConversation);
            await dataContext.SaveChangesAsync();
        }
    }

    private static async Task SeedSecurityQuestionDataAsync(DbContext dataContext, IConfiguration configuration)
    {
        var data = configuration.GetSection("SecurityQuestions").Get<string[]>();
        var sec = dataContext.Set<SecurityQuestion>();
        var questions = await sec.ToListAsync();

        foreach (var item in data)
        {
            if (questions.All(d => d.Question != item))
                sec.Add(new SecurityQuestion { Question = item });
        }

        await dataContext.SaveChangesAsync();
    }

    private static async Task SeedSubscriptionPlans(DbContext dataContext, IConfiguration configuration)
    {
        if (!await dataContext.Set<SubscriptionPlan>().AnyAsync())
        {
            dataContext.Set<SubscriptionPlan>()
                .AddRange(
                    new SubscriptionPlan
                    {
                        IsDeleted = false,
                        LastModifiedDate = null,
                        CreatedDate = DateTime.UtcNow,
                        Price = double.Parse(configuration["Stripe:Products:Supporters:Price"]),
                        AddedToCouncilGroup = false,
                        RemoveAds = false,
                        Supportbadge = true,
                        AllowDownloadPost = false,
                        DurationDays = 30,
                        IsActive = true,
                        PriceId = configuration["Stripe:Products:Supporters:Price_ID"],
                        Title = "Supporters",
                        Content = JsonConvert.SerializeObject(new SubscriptionPlanContentDto
                        {
                            Duration = "Per month",
                            Features = new[]
                            {
                                "Cancel at anytime",
                                "Support this platform to help us active our goal of ending censorship",
                                "Promoting freedom of expression"
                            }.ToList()
                        })
                    },
                    new SubscriptionPlan
                    {
                        IsDeleted = false,
                        LastModifiedDate = null,
                        CreatedDate = DateTime.UtcNow,
                        Price = double.Parse(configuration["Stripe:Products:PremiumFeatures:Price"]),
                        AddedToCouncilGroup = false,
                        RemoveAds = true,
                        Supportbadge = true,
                        AllowDownloadPost = true,
                        DurationDays = 30,
                        IsActive = true,
                        PriceId = configuration["Stripe:Products:PremiumFeatures:Price_ID"],
                        Title = "Premium Features",
                        Content = JsonConvert.SerializeObject(new SubscriptionPlanContentDto
                        {
                            Duration = "Per month",
                            Features = new[]
                            {
                                "Cancel at anytime",
                                "Use propagator add-free",
                                "Download posts",
                                "Add profile badges"
                            }.ToList()
                        })
                    },
                    new SubscriptionPlan
                    {
                        IsDeleted = false,
                        LastModifiedDate = null,
                        CreatedDate = DateTime.UtcNow,
                        Price = double.Parse(configuration["Stripe:Products:PropagatorCouncil:Price"]),
                        AddedToCouncilGroup = true,
                        RemoveAds = true,
                        Supportbadge = true,
                        AllowDownloadPost = true,
                        DurationDays = 30,
                        IsActive = true,
                        PriceId = configuration["Stripe:Products:PropagatorCouncil:Price_ID"],
                        Title = "Propagator Council",
                        Content = JsonConvert.SerializeObject(new SubscriptionPlanContentDto
                        {
                            Duration = "Per month",
                            Features = new[]
                            {
                                "Participate in exclusive discussions with the CEO",
                                "Help mold the future of platform",
                                "All Premium Features are included"
                            }.ToList()
                        })
                    });

            await dataContext.SaveChangesAsync();
        }
    }
}