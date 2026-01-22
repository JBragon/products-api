using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Entities.Products
{
    public sealed class ReturnsPolicy
    {
        public bool Allowed { get; private set; }
        public int WindowDays { get; private set; }

        protected ReturnsPolicy() { } // EF

        public ReturnsPolicy(bool allowed, int windowDays)
        {
            Allowed = allowed;
            WindowDays = windowDays;
        }
    }
}
