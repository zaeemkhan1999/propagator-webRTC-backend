namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface INotInterestedPostService : IServiceBase<NotInterestedPost, NotInterestedPostInput>
{
    Task<ResponseBase<NotInterestedPost>> AddNotInterested(NotInterestedPostInput input);
    Task<ResponseBase<NotInterestedPost>> RemoveFromNotInterestedPost(NotInterestedPostInput input);
}