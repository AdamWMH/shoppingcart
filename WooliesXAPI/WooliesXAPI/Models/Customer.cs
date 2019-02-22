using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct Customer
    {
        public int customerId;
        public List<Product> products;

        public Customer(int customerId, List<Product> products)
        {
            this.customerId = customerId;
            this.products = products;
        }
    }
}
