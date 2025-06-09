using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IAdsReadService: IServiceBase<Ads, AdsInput>
    {
        SingleResponseBase<AdsDto> GetAds(int id, User currentUser);
        ListResponseBase<AdsDto> GetAdses(User currentUser);
        ListResponseBase<AdsDto> GetAdsesForSlider(List<int> ignoredAdsIds, User currentuser);

    }
}
