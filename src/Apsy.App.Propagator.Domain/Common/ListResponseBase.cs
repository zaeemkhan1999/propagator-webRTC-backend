
using Aps.CommonBack.Base.Generics.Responses;

namespace Apsy.App.Propagator.Domain.Common
{
    public class CustomListResponseBase<T>
    {
        [UseOffsetPaging(null, 10, IncludeTotalCount = true, MaxPageSize = int.MaxValue, DefaultPageSize =500)]
        [UseProjection(11)]
        [UseFiltering(null, 12)]
        [UseSorting(null, 13)]
        public IQueryable<T> Result { get;  set; }

        [GraphQLType(typeof(AnyType))]
        public ResponseStatus Status { get; private set; }
        public int TotalCount { get; set; }
        public CustomListResponseBase(ResponseStatus status)
        {
            Status = status;
        }

        public CustomListResponseBase(IQueryable<T> result)
        {
            Result = result;
            Status = ResponseStatus.Success;
        }

        public static implicit operator CustomListResponseBase<T>(ResponseStatus status)
        {
            return new CustomListResponseBase<T>(status);
        }

        public static implicit operator ResponseStatus(CustomListResponseBase<T> listResponseBase)
        {
            return listResponseBase.Status;
        }

        public static implicit operator bool(CustomListResponseBase<T> response)
        {
            return response.Status == ResponseStatus.Success;
        }

        public static CustomListResponseBase<T> Success(IQueryable<T> result)
        {
            return new CustomListResponseBase<T>(result);
        }

        public static CustomListResponseBase<T> Failure(ResponseStatus status)
        {
            return new CustomListResponseBase<T>(status);
        }

        public static CustomListResponseBase<T> Success()
        {
            return Success(null);
        }

        public static ResponseBase<TRes> ConvertToResponseBase<TRes>(IQueryable<TRes> result)
        {
            return result.FirstOrDefault();
        }

        public void Deconstruct(out IQueryable<T> result, out ResponseStatus status)
        {
            IQueryable<T> result2 = Result;
            ResponseStatus status2 = Status;
            result = result2;
            status = status2;
        }
    }

 
}
