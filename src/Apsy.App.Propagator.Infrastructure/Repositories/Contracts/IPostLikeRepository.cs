namespace Apsy.App.Propagator.Infrastructure.Repositories;

    public interface IPostLikeRepository : IRepository<PostLikes>
    {
    IQueryable<PostLikes> GetAllPostLikes();

    IQueryable<PostLikesDto> GetPostLikesByUser(int userId);
    IQueryable<PostLikesDto> GetPostLikesUsers(int postId);

        PostLikes GetSinglePostLikesByPost(int postId, int userId);
		int GetTotalPostLikesCount();
	}

