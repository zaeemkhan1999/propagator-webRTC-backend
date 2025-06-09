namespace Apsy.App.Propagator.Application.Repositories
{
    public interface IPostLikeRepository : IRepository<PostLikes>
    {
		IQueryable<PostLikesDto> GetPostLikesByUser(int userId);
		PostLikes GetSinglePostLikesByPost(int postId, int userId);
		int GetTotalPostLikesCount();
	}
}
