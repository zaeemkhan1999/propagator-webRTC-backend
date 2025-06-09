using Microsoft.AspNetCore.Identity;

namespace Apsy.App.Propagator.Application.Services;

public class SecurityAnswerService : ServiceBase<SecurityAnswer, SecurityAnswerInput>, ISecurityAnswerService
{
    public SecurityAnswerService(ISecurityAnswerRepository repository, IHttpContextAccessor httpContextAccessor, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IUserRepository userRepository) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _userManager = userManager;
        _userRepository = userRepository;
    }
    private readonly ISecurityAnswerRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    public async Task<ListResponseBase<SecurityAnswer>> AddSecurityAnswer(List<SecurityAnswerInput> input,User currentUser)
    {
        
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (input.Count < 3)
            return ResponseStatus.NotEnoghData;

        //if (repository.Any(d=>d.UserId == currentUser.Id))
        if (repository.IsUserExist(currentUser.Id))
            return ResponseStatus.AlreadyExists;

        List<SecurityAnswer> lst = new(); 
        foreach (var item in input)
        {
            var securityAnswer = item.Adapt<SecurityAnswer>();

            securityAnswer.UserId = currentUser.Id;
            securityAnswer.SecurityQuestionId = item.QuestionId;
            lst.Add(securityAnswer);
        }

        await repository.AddRangeAsync(lst);

        ActivationTwoFactorAuthentication(currentUser.Id);

        return ListResponseBase<SecurityAnswer>.Success(lst.AsQueryable());
    }

    public async Task<ResponseBase<SecurityAnswer>> UpdateSecurityAnswer(SecurityAnswerInput input)
    {

        if (input.Id.GetValueOrDefault() == 0)
            return ResponseStatus.NotFound;

        //var cur = await repository.GetByIdAsync(input.Id);
        var cur = repository.GetSecurityAnswer(input.Id).FirstOrDefault();

        if (cur is null)
            return ResponseStatus.NotFound;
        var securityAnswer = input.Adapt(cur);
        await repository.UpdateAsync(securityAnswer);

        return ResponseBase<SecurityAnswer>.Success(securityAnswer);
    }

    private void ActivationTwoFactorAuthentication(int userId)
    {
        //var userFromDb = repository
        //                    .Where<User>(c => c.Id == userId)
        //                    .Include(x => x.AppUser)
        //                    .FirstOrDefault();

        var userFromDb = _userRepository.GetUserByIdWithAppUser(userId);

            Random _rdm = new Random();
            var code = _rdm.Next(10000, 99999);

        userFromDb.AppUser.VerificationTwoFactorCode = code.ToString();
        userFromDb.AppUser.TwoFactorEnabled = true;
       // userFromDb.TwoFactorCode = code.ToString();
        
        //else
        //{
        //    userFromDb.AppUser.TwoFactorEnabled = isActive;
        //    userFromDb.AppUser.SecurityStamp = Guid.NewGuid().ToString();
        //}

        repository.UpdateAsync<User>(userFromDb);
    }
}
