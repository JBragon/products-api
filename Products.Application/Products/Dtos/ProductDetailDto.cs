using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Products.Dtos
{
    public sealed record ProductDetailDto(
        Guid Id,
        string Title,
        string Condition,
        decimal Price,
        string Currency,
        int AvailableQuantity,
        IReadOnlyCollection<string> Pictures,
        IReadOnlyCollection<ProductAttributeDto> Attributes,
        string? Description
    );
}
