using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IUserReadService: IUserServiceBase<User, UserInput>
    {
        Task<ListResponseBase<UserClaimsViewModel>> GeteUserPermission(string username);
        ResponseBase<SingleUserDto> GetSingleUserDto(int userId);
        bool UsernameExist(User currentUser, string userName);
        ResponseBase<CurrentUserDto> GetCurrentUserFromDb();
        ResponseBase<SingleUserDto> GetUserByIdDto(int userId, int currentUserId);
        ListResponseBase<UserSusciopiousDto> GetUserLogin();
        ResponseBase<bool> IsTwoFacorEnabled();
        ResponseBase<AdminInfoDto> GetAdminInfo();
        ResponseBase<User> GetUserById(int userId);
        Task<ListResponseBase<User>> GetListUsersHaveNotBlocked();
        ListResponseBase<ResetPasswordRequest> GetResetPasswordRequests();
        string GetCurrentUsersEmail();
        string GetCurrentUsersPhoneNumber();
        string GetCurrentUsersUsername();
        Task<ResponseBase<AppUser>> GetAppUser(ClaimsPrincipal claims);
    }
}
