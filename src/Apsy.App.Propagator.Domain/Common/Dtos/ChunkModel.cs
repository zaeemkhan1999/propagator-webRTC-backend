using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ChunkModel
    {
        public string  Objectkey { get; set; }
        public List<int> ChunkNumbers { get; set; }
    }
}
