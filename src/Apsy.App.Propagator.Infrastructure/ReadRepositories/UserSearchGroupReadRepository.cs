namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchGroupReadRepository
 : Repository<UserSearchGroup,DataWriteContext>, IUserSearchGroupReadRepository
{
public UserSearchGroupReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

#endregion
#region functions
	public IQueryable<UserSearchGroup> GetUserSearchGroup()
	{
		var query = context.UserSearchGroup.AsQueryable();
		return query;

    }
    public IQueryable<User> GetUser()
    {
        var query = context.User.AsQueryable();
        return query;

    }
    public IQueryable<User> GetUserById(int id)
    {  
        var query = context.User.Where(x => x.Id == id).AsQueryable();

        return query; 
    }
    public Conversation FindConversation(int id)
    {
        return context.Conversation.Find(id);
    }
  
        #endregion
    }
