using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface ISecurityAnswerReadService: IServiceBase<SecurityAnswer, SecurityAnswerInput>
    {
        Task<ListResponseBase<SecurityAnswer>> GetSecurityAnswerCurrentUser(string username, string password, int userId = 0);
    }
}
