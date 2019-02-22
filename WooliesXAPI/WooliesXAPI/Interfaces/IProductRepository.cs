using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WooliesXAPI.Models;

namespace WooliesXAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts();

        Task<List<Customer>> GetShopperHistory();

        Task<decimal> GetTrolleyTotal(Trolley trolley);
    }
}
