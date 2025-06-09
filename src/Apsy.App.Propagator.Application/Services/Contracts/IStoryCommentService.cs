namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IStoryCommentService : IServiceBase<StoryComment, StoryCommentInput>
{
    Task<ResponseBase<StoryComment>> AddStory(StoryCommentInput input,User currentUser);
}