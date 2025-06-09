namespace Apsy.App.Propagator.Application.GraphQL.Extensions;

public class ExtendedListResponseBase<T> : ListResponseBase<T>
{
    public ExtendedListResponseBase(ResponseStatus status) : base(status) { }

    public ExtendedListResponseBase(IQueryable<T> result) : base(result) { }

    [UsePaging(IncludeTotalCount = true, MaxPageSize = int.MaxValue)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<T> Result2 => Result;

    public static implicit operator ExtendedListResponseBase<T>(ResponseStatus status)
    {
        return new ExtendedListResponseBase<T>(status);
    }

    public static implicit operator ResponseStatus(ExtendedListResponseBase<T> listResponseBase)
    {
        return listResponseBase.Status;
    }

    public static implicit operator bool(ExtendedListResponseBase<T> response)
    {
        return response.Status == ResponseStatus.Success;
    }
}

public static class ListResponseStatusExtenstions
{
    public static ExtendedListResponseBase<T> ToExtendedListResponseBase<T>(this ListResponseBase<T> listResponseBase)
    {
        if (listResponseBase.Result == null) return new(listResponseBase.Status);
        return new(listResponseBase.Result);
    }

    public static ListResponseBase<T> ToListResponseBase<T>(this ExtendedListResponseBase<T> listResponseBase)
    {
        if (listResponseBase.Result == null) return new(listResponseBase.Status);
        return new(listResponseBase.Result);
    }
}
