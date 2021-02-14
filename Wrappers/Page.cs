using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Wrappers
{
    public class Page
    {
        public int Index { get; set; }
        public int Size { get; set; }

        public Page(int index, int size)
        {
            this.Index = index < 1 ? 1 : index;
            this.Size = size > 10 ? size : 10;
        }
    }
}
