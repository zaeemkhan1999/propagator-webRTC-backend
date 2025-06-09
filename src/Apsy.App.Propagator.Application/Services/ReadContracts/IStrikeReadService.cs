using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IStrikeReadService : IServiceBase<Strike, StrikeInput>
    {
        ListResponseBase<StrikeDto> GetStrikes();
    }
}
