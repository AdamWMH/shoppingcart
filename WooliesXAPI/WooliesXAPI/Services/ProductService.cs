using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WooliesXAPI.Interfaces;
using WooliesXAPI.Models;

namespace WooliesXAPI.Services
{
    public class ProductService: IProductService
    {
        private IProductRepository productRepository;

        public ProductService(IProductRepository injectedProductRepository)
        {
            productRepository = injectedProductRepository;
        }

        public async Task<List<Product>> GetSortedProducts(string sortOption)
        {
            var products = await productRepository.GetProducts();
            switch (sortOption)
            {
                case "Low":
                    return products.OrderBy(x => x.price).ToList();
                case "High":
                    return products.OrderBy(x => x.price).Reverse().ToList();
                case "Ascending":
                    return products.OrderBy(x => x.name).ToList();
                case "Descending":
                    return products.OrderByDescending(x => x.name).ToList();
                case "Recommended":
                    var shopperHistory = await productRepository.GetShopperHistory();
                    var pivotArray = shopperHistory.SelectMany(x => x.products).ToList().GroupBy(x => x.name).Select(x => new { Key = x.Key, TotalSold = x.Sum(y => y.quantity) }).OrderByDescending(x => x.TotalSold);
                    var sortedList = new List<Product>();
                    foreach (var product in pivotArray)
                    {
                        sortedList.Add(products.FirstOrDefault(x => x.name == product.Key));
                    }
                    foreach (var unsoldProduct in products.Where(x => !sortedList.Any(y => y.name == x.name)))
                    {
                        sortedList.Add(unsoldProduct);
                    }
                    return sortedList;
            }
            return null;
        }

        public async Task<decimal> GetTrolleyTotal(Trolley trolley)
        {
            var total = await productRepository.GetTrolleyTotal(trolley);
            return total;
        }
    }
}
