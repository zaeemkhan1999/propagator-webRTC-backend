namespace Apsy.App.Propagator.Application.Services;

public class ReportService : ServiceBase<Report, ReportInput>, IReportService
{
    public ReportService(IReportRepository repository) : base(repository)
    {
        this.repository = repository;
    }

    private readonly IReportRepository repository;

    public override ResponseBase<Report> Add(ReportInput input)
    {
        if (input.ReportType == ReportType.Post && input.PostId == null)
            return ResponseStatus.NotEnoghData;
        if (input.ReportType == ReportType.Article && input.ArticleId == null)
            return ResponseStatus.NotEnoghData;
        if (input.ReportType == ReportType.PostComment && input.CommentId == null)
            return ResponseStatus.NotEnoghData;
        if (input.ReportType == ReportType.ArticleComment && input.ArticleCommentId == null)
            return ResponseStatus.NotEnoghData;
        if (input.ReportType == ReportType.OtherUser && input.ReportedId == null)
            return ResponseStatus.NotEnoghData;
        if (input.ReportType == ReportType.Message && input.MessageId == null)
            return ResponseStatus.NotEnoghData;

        return base.Add(input);
    }
}