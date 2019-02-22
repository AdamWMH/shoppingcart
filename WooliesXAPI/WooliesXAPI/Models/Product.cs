using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct Product
    {
        public string name;
        public decimal? price, quantity;
        
        public Product(string name, decimal? price, decimal? quantity)
        {
            this.name = name;
            this.price = price;
            this.quantity = quantity;
        }
    }
}
