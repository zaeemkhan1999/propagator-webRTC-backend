namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IPostLikeReadService : IServiceBase<PostLikes, PostLikeInput>
    {
        ListResponseBase<PostLikesDto> GetPostLikes();
        ListResponseBase<PostLikesDto> GetPostLikesUsers(int postId);
    }
}