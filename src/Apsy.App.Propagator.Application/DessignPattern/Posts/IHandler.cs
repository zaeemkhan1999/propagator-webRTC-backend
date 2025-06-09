namespace Apsy.App.Propagator.Application.DessignPattern.Posts;

// The Handler interface declares a method for building the chain of
// handlers. It also declares a method for executing a request.
public interface IHandler
{
    IHandler SetNext(IHandler handler);

    ListResponseBase<PostDto> Handle(object request,User currentUser);
}
