namespace Apsy.App.Propagator.Domain.Entities
{

    public class WarningBanner : UserKindDef<User>
    {
        public string Description { get; set; }
        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }


        [GraphQLIgnore]
        public List<BaseEvent> RaiseEvent(ref List<BaseEvent> events, User currrentUser, CrudType crudType)
        {
            if (crudType == CrudType.AddWarningAsBannerEvent)
            {
                var addWarningAsBannerSetEvent = new AddWarningAsBannerSetEvent()
                {
                    AdminId = currrentUser.Id,
                    UserId = UserId,
                    Description = Description,

                    PostId = PostId,
                    YourMind = Post?.YourMind,
                    PostOwnerEmail = Post?.Poster?.Email,
                    PostOwnerId = Post?.PosterId,
                    ArticleId = ArticleId,
                    ArticleOwnerId = Article?.UserId,
                    UserDisplayName = User?.DisplayName,
                    UserImageAddress = User?.ImageAddress,
                    UserCover = User?.Cover,
                    UserName = User?.Username,
                    UserEmail = User?.Email
                };
                events.Add(addWarningAsBannerSetEvent);
            }
            else if (crudType == CrudType.DeleteWarningAsBannerEvent)
            {
                var deleteWarningAsBannerSetEvent = new DeleteWarningAsBannerSetEvent()
                {
                    AdminId = currrentUser.Id,
                    UserId = UserId,
                    Description = Description,
                    PostId = PostId,
                    YourMind = Post?.YourMind,
                    PostOwnerEmail = Post?.Poster?.Email,
                    PostOwnerId = Post?.PosterId,
                    ArticleId = ArticleId,
                    ArticleOwnerId = Article?.UserId,
                    UserDisplayName = User?.DisplayName,
                    UserImageAddress = User?.ImageAddress,
                    UserCover = User?.Cover,
                    UserName = User?.Username,
                    UserEmail = User?.Email

                };
                events.Add(deleteWarningAsBannerSetEvent);
            }

            return events;
        }
    }
}