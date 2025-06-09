namespace Apsy.App.Propagator.Application.Common
{
    public class PostExploreDto : DtoDef
    {
        public PostExploreDto(ResponseStatus status)
        {
            Status = status;
        }

        public PostExploreDto(IQueryable<PostDto> result,
            int totalCount,
            bool nextPage,
            bool hasPreviousPage)
            : this(ResponseStatus.Success)
        {
            Result = result;
            TotalCount = totalCount;
            HasNextPage = nextPage;
            HasPreviousPage = hasPreviousPage;
            Status = ResponseStatus.Success;
        }

        [UseProjection(11)]
        public IQueryable<PostDto> Result { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        [GraphQLType(typeof(AnyType))]
        public ResponseStatus Status { get; private set; }

        public static implicit operator PostExploreDto(ResponseStatus status)
        {
            return new PostExploreDto(status);
        }
    }
}
