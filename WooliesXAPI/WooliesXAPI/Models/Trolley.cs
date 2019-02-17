using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct Trolley
    {
        public List<Product> products;
        public List<Special> specials;
        public List<ProductQuantity> quantities;

        public Trolley(List<Product> products, List<Special> specials, List<ProductQuantity> quantities)
        {
            this.products = products;
            this.specials = specials;
            this.quantities = quantities;
        }
    }
}
