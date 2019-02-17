using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct Special
    {
        public List<ProductQuantity> quantities;
        public decimal total;

        public Special(List<ProductQuantity> quantities, decimal total)
        {
            this.quantities = quantities;
            this.total = total;
        }
    }
}
