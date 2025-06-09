namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class StoryMutations
{
    [GraphQLName("story_createStory")]
    public ResponseBase<Story> Create(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStoryService service,
        StoryInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        var response = service.Add(input);
        if (response != null)
        {
            service.redisStory(authentication.CurrentUser.Id);
            service.SetHasStory(currentUser.Id);

        }
        return response;
    }
    [GraphQLName("story_updateStory")]
    public ResponseBase<Story> Update(
    [Authentication] Authentication authentication,
    [Service(ServiceKind.Default)] IStoryService service,
    StoryInput input)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }


        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        var response = service.Update(input);
        if (response != null)
        {
            service.redisStory(authentication.CurrentUser.Id);
        }
        return response;
    }


    [GraphQLName("story_removeStory")]
    public ResponseStatus Remove(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStoryService service,
        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        var response = service.SoftDelete(entityId);
        if (response != null)
        {
            service.redisStory(authentication.CurrentUser.Id);
            
            service.CheckStories(authentication.CurrentUser.Id);
        }
        return response;
    }

    [GraphQLName("story_removeListStory")]
    public ResponseBase<bool> RemoveList(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IStoryService service,
        List<int> ids)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var response = service.SoftDeleteAll(ids, authentication.CurrentUser);
        if (response != null)
        {
            service.redisStory(authentication.CurrentUser.Id);
        }
        return response;    
    }

    [GraphQLName("story_addStoryToHighLigh")]
    public ResponseBase<Story> AddStoryToHighLigh(
                              [Authentication] Authentication authentication,
                              [Service(ServiceKind.Default)] IStoryService service,
                              int storyId,
                              int highLLightId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.AddSotyToHighLigh(storyId, highLLightId, authentication.CurrentUser);
    }

    [GraphQLName("story_removeStoryFromHighLigh")]
    public ResponseBase<Story> RemoveStoryFromHighLigh(
                              [Authentication] Authentication authentication,
                              [Service(ServiceKind.Default)] IStoryService service,
                              int storyId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.RemoveStoryFromHighLigh(storyId, authentication.CurrentUser);
    }

    [GraphQLName("story_likeStory")]
    public async Task<ResponseBase<StoryLike>> LikeStory(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        int storyId,
        bool liked = true)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        return await service.LikeStory(currentUser.Id, storyId, liked,authentication.CurrentUser);
    }

    [GraphQLName("story_unLikeStory")]
    public ResponseBase UnLikeStory(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        int storyId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return service.UnLikeStory(currentUser.Id, storyId);
    }

    [GraphQLName("story_hideStories")]
    public ListResponseBase<HideStory> HideStories(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        List<int> userIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.HideStories(userIds, authentication.CurrentUser);
    }

    [GraphQLName("story_hideStory")]
    public ResponseBase<HideStory> HideStory(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.HideStory(userId, authentication.CurrentUser);
    }

    [GraphQLName("story_unHideStory")]
    public ResponseBase UnHideStory(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.UnHideStory(userId, authentication.CurrentUser);
    }

    [GraphQLName("story_unHideStories")]
    public ListResponseBase<HideStory> UnHideStories(
        [Authentication] Authentication authentication,
        [Service] IStoryService service,
        List<int> userIds)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.UnHideStories(userIds, authentication.CurrentUser);
    }

    [GraphQLName("story_createComment")]
    public async Task<ResponseBase<StoryComment>> CreateComment(
        [Authentication] Authentication authentication,
              [Service] IStoryCommentService storyCommentService,
              StoryCommentInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        input.UserId = currentUser.Id;
        return await storyCommentService.AddStory(input, currentUser);
    }

    [GraphQLName("store_undoStoreRemove")]
    public async Task<ResponseStatus> UndoStoreRemove(
              [Authentication] Authentication authentication,
              [Service] IStoryService service,
              int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        if (authentication.CurrentUser.UserTypes != UserTypes.Admin && authentication.CurrentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        return await service.UndoDeleteStore(entityId);
    }
}
