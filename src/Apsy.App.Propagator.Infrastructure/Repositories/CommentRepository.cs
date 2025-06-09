namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment, DataReadContext>, ICommentRepository
{
    public CommentRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataReadContext context;

    
    public IQueryable<CommentDto> GetComment(int Id,int UserId)
    {
        return context.Comment.Where(x=>x.Id==Id).Select(c => new CommentDto()
        {
            Comment = c,
            IsLiked = c.LikeComments.Any(c => c.UserId == UserId),
            LikeCount = c.LikeComments.Count,
            HasChild = c.Children.Any(),
            ChildrenCount = c.Children.Count
        }).AsNoTracking().AsQueryable();
    }
    public IQueryable<Comment> GetAllComments()
    {
        var query = context.Comment.AsQueryable();
        return query;
    }

    public IQueryable<CommentDto> GetComments(User currentUser)
    {
        return  context.Comment.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.UserId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Select(c => new CommentDto
            {
                Comment = c,
                IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                LikeCount = c.LikeComments.Count,
                HasChild = c.Children.Any(),
                ChildrenCount = c.Children.Count,
                Children = c.Children.Select(child => new CommentDto
                {
                    Comment = child,
                    IsLiked = child.LikeComments.Any(lc => lc.UserId == currentUser.Id),
                    LikeCount = child.LikeComments.Count,
                    HasChild = child.Children.Any(),
                    ChildrenCount = child.Children.Count
                }).ToList()
            });
    }
    public Comment GetComment(int Id)
    { 
         return context.Comment.Include(c => c.Post)
                .FirstOrDefault(x=>x.Id==Id);
         
    }

    public IQueryable<Comment> GetComments(List<int> ids, User currentUser)
    {
        return context.Comment.Where(r => ids.Contains(r.Id)).Include(c => c.Post).AsQueryable();
    }

    public int GetCommentCount(int Id)
    {
        return context.Comment.Where(d => d.PostId == Id && d.IsDeleted == false && d.DeletedBy == DeletedBy.NotDeleted).Count();
    }
    public int GetCommentCount()
    {
        return context.Comment.Where(d => d.IsDeleted == false && d.DeletedBy == DeletedBy.NotDeleted).Count();
    }
    public IQueryable<Comment> GetCommentByPostId(int postId)
    {
        return context.Comment.Where(c => c.PostId == postId).AsNoTracking().AsQueryable();
    }

   

    public int GetTotalCommentsCount()
    {
        return context.Comment.Where(d => d.IsDeleted == false && d.DeletedBy == DeletedBy.NotDeleted).AsNoTracking().Count();
    }

    public int GetPostTotalCommentsCount(int postId)
    {
        return context.Comment.Where(d => d.PostId == postId && d.IsDeleted == false && d.DeletedBy == DeletedBy.NotDeleted).AsNoTracking().Count();
    }

    public bool isUserBlockForPostComment(int userId, int posterId)
    {
        return context.BlockUser.Any(x => x.BlockerId == posterId && x.BlockedId == userId);
    
    }

    public async Task<List<Comment>> GetReplies(int parentCommentId)
    {
        return await context.Comment
            .Where(c => c.ParentCommentId == parentCommentId && 
                    c.DeletedBy == DeletedBy.NotDeleted)
            .ToListAsync();
    }
}