using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IUserSearchGroupReadService:IServiceBase<UserSearchGroup, UserSearchGroupInput>
    {
        SingleResponseBase<UserSearchGroupDto> GetUserSearchGroup(int id);
        ListResponseBase<UserSearchGroupDto> GetUserSearchGroups(User currentUser);
    }
}
