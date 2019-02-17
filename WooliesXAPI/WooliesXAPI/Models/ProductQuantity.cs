using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct ProductQuantity
    {
        public string name;
        public int quantity;

        public ProductQuantity(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
        }
    }
}
