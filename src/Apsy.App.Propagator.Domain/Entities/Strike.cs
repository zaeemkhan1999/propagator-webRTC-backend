namespace Apsy.App.Propagator.Domain.Entities
{
    public class Strike : UserKindDef<User>
    {
        public string Text { get; set; }

        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }

        [GraphQLIgnore]
        public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, bool isAdded)
        {
            if (isAdded)
            {

                var strikeGivenEvent = new StrikeGivenEvent()
                {
                    AdminId = currrentUser.Id,
                    UserId = UserId,
                    UserEmail = User.Email,
                    Text = Text,

                    PostId = PostId,
                    PostOwnerId = Post?.PosterId,
                    YourMind = Post?.YourMind,

                    ArticleId = ArticleId,
                    ArticleOwnerId = Article?.UserId,
                    UserImageAddress = User?.ImageAddress,
                    UserCover = User?.Cover,
                    UserDisplayName = User?.DisplayName,
                    UserName = User?.Username
                };
                events.Add(strikeGivenEvent);
            }
            else
            {
                var strikeGivenEvent = new RemoveStrikeGivenEvent()
                {
                    AdminId = currrentUser.Id,
                    UserId = UserId,
                    UserEmail = User.Email,
                    Text = Text,

                    PostId = PostId,
                    PostOwnerId = Post?.PosterId,
                    YourMind = Post?.YourMind,

                    ArticleId = ArticleId,
                    ArticleOwnerId = Article?.UserId,
                    UserImageAddress = User?.ImageAddress,
                    UserCover = User?.Cover,
                    UserDisplayName = User?.DisplayName,
                    UserName = User?.Username
                };
                events.Add(strikeGivenEvent);
            }
            return events;
        }
    }
}