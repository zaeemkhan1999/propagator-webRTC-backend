namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchPlaceService : IServiceBase<UserSearchPlace, UserSearchPlaceInput>
{
    ResponseBase<UserSearchPlace> DeleteSearchedPlace(int userId, string place);
}