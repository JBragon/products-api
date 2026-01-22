using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Products.Dtos
{
    public sealed record RatingDto(
         decimal Average,
         int TotalReviews
     );
}
