using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Entities.Products
{
    public sealed class ProductPicture
    {
        public string Url { get; private set; } = string.Empty;

        protected ProductPicture() { } // EF

        public ProductPicture(string url)
        {
            Url = url;
        }
    }
}
