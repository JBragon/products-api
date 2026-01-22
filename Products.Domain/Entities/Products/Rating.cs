using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Entities.Products
{
    public sealed class Rating
    {
        public decimal Average { get; private set; }
        public int TotalReviews { get; private set; }

        protected Rating() { } // EF

        public Rating(decimal average, int totalReviews)
        {
            Average = average;
            TotalReviews = totalReviews;
        }
    }
}
