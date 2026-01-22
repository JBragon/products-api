using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Common
{
    public sealed record PagedResult<T>(
        IReadOnlyCollection<T> Items,
        int Page,
        int PageSize,
        int TotalItems
    );
}
