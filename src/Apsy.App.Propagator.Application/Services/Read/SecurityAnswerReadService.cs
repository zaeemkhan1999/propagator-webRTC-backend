using Aps.CommonBack.Base.Repositories;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class SecurityAnswerReadService: ServiceBase<SecurityAnswer, SecurityAnswerInput>, ISecurityAnswerReadService
    {
        private readonly ISecurityAnswerReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserReadRepository _userRepository;
        public SecurityAnswerReadService(ISecurityAnswerReadRepository repository, IHttpContextAccessor httpContextAccessor, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IUserReadRepository userRepository) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
        }
        public async Task<ListResponseBase<SecurityAnswer>> GetSecurityAnswerCurrentUser(string username, string password, int userId = 0)
        {
            if (userId == 0)
            {

                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return ResponseStatus.UserNotFound;

                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);

                if (!signInResult.RequiresTwoFactor)
                    return ResponseStatus.UserNotFound;

                userId = user.UserId;
            }
            var query = repository.GetSecurityAnswerByUserId(userId);
            return ListResponseBase<SecurityAnswer>.Success(query);
        }
    }
}
