using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigreh.Wrappers;

namespace sigreh.Services
{
    public class PaginatorService
    {
        public static PageResponse<List<T>> Paginate<T>(List<T> data, int total, Page page)
        {
            var result = new PageResponse<List<T>>();
            result.Total = Convert.ToInt32(Math.Ceiling(((double)total / (double)page.Size)));
            result.Next = page.Index >= 1 && page.Index < result.Total ? new Page(page.Index + 1, page.Size) : null;
            result.Previous = page.Index - 1 >= 1 && page.Index <= result.Total ? new Page(page.Index - 1, page.Size) : null;
            result.First = new Page(1, page.Size);
            result.Last = new Page(result.Total, page.Size);
            result.Data = data;
            return result;
        }
    }
}
