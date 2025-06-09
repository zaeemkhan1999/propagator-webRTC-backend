using Mapster;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class MessageReadRepository
 : MessageRepositoryBase<DataWriteContext, Message, Conversation, User>, IMessageReadRepository
{
    public MessageReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataWriteContext context;
    public Conversation GetConversation(int Id)
    {
        return context.Conversation.Where(x => x.Id == Id && x.IsGroup).Include(x => x.UserGroups).FirstOrDefault();
    }
    public IQueryable<Conversation> GetConversation(int Id, string Grouplink)
    {
        return context.Conversation.Where(x => x.Id == Id && x.GroupLink.Equals(Grouplink));
    }
    public IQueryable<Conversation> ConversationGetAll()
    {
        return context.Conversation.AsNoTracking();
    }
    public IQueryable<MessageDto> GetMessages(int conversationid,User currentUser)
    {
       return  context.Message.Where(c =>
                    !c.Conversation.IsGroup && c.ConversationId == conversationid &&
                          (c.SenderId == currentUser.Id || c.ReceiverId == currentUser.Id))
                .Include(c => c.Post)
                .ThenInclude(c => c.Poster)
                .Include(c => c.Article)
                .ThenInclude(c => c.User)
                .Include(c => c.Profile)
                .Include(c => c.Story)
                .ThenInclude(c => c.User)
                .Include(c => c.Story.Post)
                .Include(c => c.Receiver)
                .Include(c => c.ParentMessage)
                .ProjectToType<MessageDto>();
    }
    public IQueryable<Message> GetMessages()
    {
        return context.Message;
    }
    public IQueryable<ConversationDto> GetConversationGroups(int UserId)
    {
        return context.Conversation.Where(a => a.IsGroup)
                    .Select(c => new ConversationDto
                    {
                        ConversationId = c.Id,
                        LatestMessageDate = c.LatestMessageDate,
                        UnreadCount = c.UserGroups.FirstOrDefault(x => x.UserId == UserId) != null ? c.UserGroups.FirstOrDefault(x => x.UserId == UserId).UnreadCount : 0,

                        LastMessage = c.Messages
                                         .Where(c => c.SenderId == UserId || c.ReceiverId == UserId)
                                         .OrderBy(x => x.CreatedDate)
                                         .LastOrDefault(),
                        IsGroup = c.IsGroup,
                        GroupName = c.GroupName,
                        GroupImgageUrl = c.GroupImgageUrl,

                        GroupDescription = c.GroupDescription,

                        LatestMessageUserId = c.LatestMessageUserId,

                        GroupMemberCount = c.UserGroups.Count,

                        GroupLink = c.GroupLink,
                        IsPrivate = c.IsPrivate,
                        AdminId = c.AdminId,
                        IsMemberOfGroup = c.UserGroups.Any(x => x.UserId == UserId)
                    });
    }
    public IQueryable<UserMessageGroup> GetUserMessageGroups(int Id, int Userid)
    {
        return context.UserMessageGroup.Where(x => x.ConversationId == Id && x.UserId == Userid);
    }

    public IQueryable<UserMessageGroup> GetUserMessageGroupsByConversationAndUserId(int userId, int conversationId)
    {
        return context.UserMessageGroup.Where(x => x.ConversationId == conversationId && x.UserId == userId).AsNoTracking().AsQueryable();
    }
    public IQueryable<UserMessageGroup> GetUserMessageGroups(int Id)
    {
        return context.UserMessageGroup.Where(x => x.ConversationId == Id && x.Conversation.IsGroup);
    }
    public IQueryable<UserMessageGroup> GetUserMessageGroupsWithoutIsgroup(int Id)
    {
        return context.UserMessageGroup.Where(x => x.ConversationId == Id);
    }
    public IQueryable<Message> GetMessagesByGroupTopic(int Id)
    {
        return context.Message.Where(x => x.GroupTopicId == Id);
    }
    public IQueryable<Message> GetMessages(int Id, int UserId)
    {
        return context.Message.Where(c => c.ConversationId == Id
                                        &&
                                        (c.SenderId == UserId || c.ReceiverId == UserId));
    }
    public IQueryable<Message> GetMessages(List<int> messagesIds)
    {
        return context.Message.Where(c => messagesIds.Contains(c.Id));
    }
    public IQueryable<Message> GetMessages(int Id, bool checkeddelete = false)
    {
        if (checkeddelete)
        {
            return context.Message.IgnoreQueryFilters().Where(x => x.Id == Id && x.IsDeleted);
        }
        return (IQueryable<Message>)context.Message.Find(Id);
    }
    public IQueryable<DiscussionsDto> GetDiscussions(User currentUser)
    {
        return context.Message.Include(x=>x.Post).ThenInclude(x=>x.Poster)
              .Where<Message>(c => c.MessageType == MessageType.Article || c.MessageType == MessageType.Post)
              .Select(c => new DiscussionsDto
              {
                  Id = c.Id,
                  ConversationId = c.ConversationId,
                  CreatedDate = c.CreatedDate,
                  IsDeleted = c.IsDeleted,
                  LastModifiedDate = c.LastModifiedDate,
                  MessageType = c.MessageType,
                  GroupTopicId = c.GroupTopicId,
                  Post = c.Post,
                  
                  Article = c.Article,
                  ItemsString = IsPost(c) ? (c.Post != null ? c.Post.PostItemsString : null) : (c.Article != null ? c.Article.ArticleItemsString : null),
                  IsLiked = IsPost(c) ? (c.Post != null ? c.Post.Likes.Any(l => l.UserId == currentUser.Id && l.Liked) : false) : (c.Article != null ? c.Article.ArticleLikes.Any(l => l.UserId == currentUser.Id) : false),
                  IsViewed = IsPost(c) ? (c.Post != null ? c.Post.UserViewPosts.Any(v => v.UserId == currentUser.Id) : false) : (c.Article != null ? c.Article.UserViewArticles.Any(v => v.UserId == currentUser.Id) : false),
                  IsNotInterested = IsPost(c) ? (c.Post != null ? c.Post.NotInterestedPosts.Any(n => n.UserId == currentUser.Id) : false) : (c.Article != null ? c.Article.NotInterestedArticles.Any(n => n.UserId == currentUser.Id) : false),
                  IsSaved = IsPost(c) ? (c.Post != null ? c.Post.SavePosts.Any(s => s.UserId == currentUser.Id) : false) : (c.Article != null ? c.Article.SaveArticles.Any(s => s.UserId == currentUser.Id) : false),
                  IsYours = IsPost(c) ? (c.Post != null ? c.Post.PosterId == currentUser.Id : false) : (c.Article != null ? c.Article.UserId == currentUser.Id : false),
                  CommentCount = IsPost(c) ? (c.Post != null ? c.Post.Comments.Count : 0) : (c.Article != null ? c.Article.ArticleComments.Count : 0),
                  ShareCount = IsPost(c) ? (c.Post != null ? c.Post.Messages.Count : 0) : (c.Article != null ? c.Article.Messages.Count : 0),
                  LikeCount = IsPost(c) ? (c.Post != null ? c.Post.LikesCount : 0) : (c.Article != null ? c.Article.ArticleLikes.Count : 0),
                  ViewCount = IsPost(c) ? (c.Post != null ? c.Post.UserViewPosts.Count : 0) : (c.Article != null ? c.Article.UserViewArticles.Count : 0),
              }).AsQueryable();
    }
    private static bool IsPost(Message message) => message.MessageType == MessageType.Post;

    public IQueryable<Conversation> GetConversation(int otherUserId, User currentUser)
    {
        return context.Conversation
           .Where(c => !c.IsGroup &&
           (
            (c.FirstUserId == currentUser.Id && c.SecondUserId == otherUserId)
            ||
            (c.SecondUserId == currentUser.Id && c.FirstUserId == otherUserId)
            ));
    }

    public IQueryable<FollowUserForGroupDto> GetUsersNotInvitedToGroup(int? conversationId, User currentUser)
    {
        return context.UserFollower.Where(x =>
                x.Follower.Blocks.All(y => y.BlockedId != currentUser.Id) &&
                x.Followed.Blocks.All(y => y.BlockedId != x.FollowerId))
            .Select(x => new FollowUserForGroupDto
            {
                User = x.Followed,
                IsMemberOfGroup = conversationId != null &&
                                  x.Follower.UserMessageGroups.Any(y =>
                                      y.ConversationId == conversationId &&
                                      y.Conversation.UserGroups.Any(z => z.UserId == x.FollowedId))
            }).AsNoTracking().AsQueryable();
    }

    public IQueryable<FollowUserForGroupDto> GetMyFollowerNotInvitedToGroup(int? conversationId, User currentUser)
    {
        var Followers = context.UserFollower.Where(c =>
                                c.FollowerId == currentUser.Id &&
                                !c.IsMutual &&
                                c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted &&

                                !c.Followed.Blocks.Any(x => x.BlockedId == currentUser.Id) &&
                                !c.Follower.Blocks.Any(x => x.BlockedId == c.FollowedId));

        var myfollowings = Followers.Select(wp => new FollowUserForGroupDto
        {
            User = wp.Followed,
            IsMemberOfGroup = conversationId == null ? false : wp.Follower.UserMessageGroups.Any(x => x.ConversationId == conversationId && x.Conversation.UserGroups.Any(z => z.UserId == wp.FollowedId))
        });

        var myFollowers = Followers.Select(wp => new FollowUserForGroupDto
        {
            User = wp.Follower,
            IsMemberOfGroup = conversationId == null ? false : wp.Followed.UserMessageGroups.Any(x => x.ConversationId == conversationId && x.Conversation.UserGroups.Any(z => z.UserId == wp.FollowerId))
        });

        var bothFolowerAndFollowing = Followers.Select(wp => new FollowUserForGroupDto
        {
            User = wp.Follower,
            IsMemberOfGroup = conversationId == null ? false : wp.Followed.UserMessageGroups.Any(x => x.ConversationId == conversationId && x.Conversation.UserGroups.Any(z => z.UserId == wp.FollowerId))
        });

        var unionUsers = myfollowings.Union(myFollowers).Union(bothFolowerAndFollowing).OrderBy(c => c.User.CreatedDate);
        return unionUsers;
    }

    public IQueryable<ConversationDto> GetConversations(int userId)
    {

        var res= context.Conversation.Where(c => c.IsGroup || (c.FirstUserId == userId || c.SecondUserId == userId))
                .Select(c => new ConversationDto
                {
                    ConversationId = c.Id,
                    LatestMessageDate = c.LatestMessageDate,
                    UnreadCount = (!c.IsGroup) ?
                                         (c.FirstUserId == userId ? c.FirstUnreadCount : c.SecondUnreadCount) :
                                         c.UserGroups.FirstOrDefault(x => x.UserId == userId) == null
                                             ? 0
                                             : c.UserGroups.FirstOrDefault(x => x.UserId == userId).UnreadCount,
                    LastMessage = c.Messages
                                         .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                                         .OrderBy(x => x.CreatedDate)
                                         .LastOrDefault(),
                    IsGroup = c.IsGroup,
                    GroupName = c.GroupName,
                    GroupImgageUrl = c.GroupImgageUrl,

                    IsFirstUserDeleted = c.IsFirstUserDeleted,
                    FirstUserDeletedDate = c.FirstUserDeletedDate,

                    IsSecondUserDeleted = c.IsSecondUserDeleted,
                    SecondUserDeletedDate = c.SecondUserDeletedDate,
                    LatestMessageUserId = c.LatestMessageUserId,

                    UserId = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.Id : c.FirstUser.Id,
                    Email = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.Email : c.FirstUser.Email,
                    DisplayName = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.DisplayName : c.FirstUser.DisplayName,
                    ImageAddress = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.ImageAddress : c.FirstUser.ImageAddress,
                    Cover = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.Cover : c.FirstUser.Cover,
                    UserTypes = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.UserTypes : c.FirstUser.UserTypes,
                    Username = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.Username : c.FirstUser.Username,
                    LastSeen = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.LastSeen : c.FirstUser.LastSeen,
                    Bio = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.Bio : c.FirstUser.Bio,
                    DateOfBirth = c.IsGroup ? null : c.FirstUserId == userId ? c.SecondUser.DateOfBirth : c.FirstUser.DateOfBirth,
                });
        var dateThreshold = DateTime.Now.AddDays(-30);

        var expiredmessages = context.Message.Where(x => res.Select(z => z.ConversationId).ToList().Contains(x.ConversationId) && x.CreatedDate <= dateThreshold).ToList();
        // Your long-running or background task here
        if (expiredmessages.Count() > 0)
        {
            context.Message.RemoveRange(expiredmessages);
            context.SaveChanges();
        }



        return res;
    }

    public IQueryable<UserMessageGroup> GetUserMessageGroupsNextAdmin(int conversationId, UserMessageGroup Member)
    {
        return context.UserMessageGroup.Where(x => x.ConversationId == conversationId && x.Id != Member.Id)
                                        .OrderBy(x => Math.Abs((x.CreatedDate - Member.CreatedDate).Ticks));

    }
    public IQueryable<UserFollower> GetUserFollowers(int FollowerId, int FollowedId)
    {
        return context.UserFollower.Where(x => x.FollowerId == FollowerId && x.FollowedId == FollowedId);
    }
    public IQueryable<ConversationDto> GroupList(int conversationId, User currentUser)
    {

        return context.Conversation.Where(x => x.Id == conversationId).
           Select(c => new ConversationDto()
           {
               ConversationId = c.Id,
               LatestMessageDate = c.LatestMessageDate,
               UnreadCount = c.UserGroups.FirstOrDefault(x => x.UserId == currentUser.Id) != null ? c.UserGroups.FirstOrDefault(x => x.UserId == currentUser.Id).UnreadCount : 0,

               LastMessage = c.Messages
                                        .Where(c => c.SenderId == currentUser.Id || c.ReceiverId == currentUser.Id)
                                        .OrderBy(x => x.CreatedDate)
                                        .LastOrDefault(),
               IsGroup = c.IsGroup,
               GroupName = c.GroupName,
               GroupImgageUrl = c.GroupImgageUrl,
               GroupDescription = c.GroupDescription,
               LatestMessageUserId = c.LatestMessageUserId,
               GroupMemberCount = c.UserGroups.Count,
               GroupLink = c.GroupLink,
               IsPrivate = c.IsPrivate,
               AdminId = c.AdminId,
               IsMemberOfGroup = c.UserGroups.Any(x => x.UserId == currentUser.Id)
           });

    }

    public Conversation GetConversationBySubscriptionPlanId(int subscriptionPlanId)
    {
        return context.Conversation.Where(x => x.SubscriptionPlanId == subscriptionPlanId).AsNoTracking().FirstOrDefault();
    }
    public IQueryable<Conversation> GetMessageConversations()
    {
        var query = context.Conversation.AsQueryable();
        return query;
    }
    public IQueryable<UserMessageGroup> GetMessageGroups()
    {
        var query = context.UserMessageGroup.AsQueryable();
        return query;
    }

    public async Task<Conversation> GetConversationById(int id)
    {
        var query = await context.Conversation.FindAsync(id);
        return query;
    }

    #endregion
}
