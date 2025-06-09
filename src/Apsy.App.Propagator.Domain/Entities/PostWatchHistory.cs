using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Domain.Entities
{
    public class PostWatchHistory
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DeletedBy DeletedBy { get; set; }
        public DateTime WatchDate { get; set; }

    }
}