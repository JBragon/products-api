using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Entities.Products
{
    public sealed class ProductHighlight
    {
        public string Text { get; private set; } = string.Empty;

        protected ProductHighlight() { } // EF

        public ProductHighlight(string text)
        {
            Text = text;
        }
    }
}
