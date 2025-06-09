using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IArticleCommentReadService : IServiceBase<ArticleComment, ArticleCommentInput>
    {
        SingleResponseBase<ArticleCommentDto> GetArticleComment(int id, User currentUser);
        ListResponseBase<ArticleCommentDto> GetArticleComments(bool loadDeleted, User currentUser);
    }
}
