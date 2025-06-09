using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface ICommentReadService: IServiceBase<Comment, CommentInput>
    {
        SingleResponseBase<CommentDto> GetComment(int id, User currentUser);
        ListResponseBase<CommentDto> GetComments(bool loadDeleted, User currentUser);

    }
}
