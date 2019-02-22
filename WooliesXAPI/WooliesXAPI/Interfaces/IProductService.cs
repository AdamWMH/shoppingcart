using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WooliesXAPI.Models;

namespace WooliesXAPI.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetSortedProducts(string sortOption);

        Task<decimal> GetTrolleyTotal(Trolley trolley);

        Task<decimal> CalculateTrolleyTotal(Trolley trolley);
    }
}
