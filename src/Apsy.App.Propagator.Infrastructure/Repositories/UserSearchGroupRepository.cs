namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserSearchGroupRepository
 : Repository<UserSearchGroup,DataReadContext>,IUserSearchGroupRepository
{
public UserSearchGroupRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

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
