namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IHighLightService : IServiceBase<HighLight, HighLightInput>
{
     ResponseBase<HighLight> Add(HighLightInput input, User currentUser);
    ResponseBase<HighLight> Update(HighLightInput input, User currentUser);
    ResponseStatus SoftDelete(int entityId, User currentUser);
}

