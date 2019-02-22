using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WooliesXAPI.Interfaces;
using WooliesXAPI.Models;

namespace WooliesXAPI.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository productRepository;

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
                    var pivotArray = shopperHistory.SelectMany(x => x.products).ToList().GroupBy(x => x.name).Select(x => new {x.Key, TotalSold = x.Sum(y => y.quantity) }).OrderByDescending(x => x.TotalSold);
                    var sortedList = new List<Product>();
                    foreach (var product in pivotArray)
                    {
                        sortedList.Add(products.FirstOrDefault(x => x.name == product.Key));
                    }
                    foreach (var unsoldProduct in products.Where(x => sortedList.All(y => y.name != x.name)))
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

        public Task<decimal> CalculateTrolleyTotal(Trolley trolley)
        {
            var specialCombos = GetAllCombos(trolley.specials);
            var fullTotal = trolley.quantities.Sum(q => trolley.products.First(p => p.name == q.name).price * q.quantity);
            var lowestPrice = fullTotal;
            foreach (var combo in specialCombos)
            {
                var invalidTotal = false;
                if (trolley.quantities.Any(t =>
                    combo.Sum(c => c.quantities.Where(q => q.name == t.name).Sum(y => y.quantity)) > t.quantity))
                {
                    continue;
                }

                var totalPrice = combo.Sum(c => c.total);

                if (totalPrice >= lowestPrice)
                {
                    continue;
                }

                foreach (var product in trolley.quantities)
                {
                    var remainingRequired = product.quantity - combo.Sum(c => c.quantities.Where(q => q.name == product.name).Sum(s => s.quantity));
                    if (remainingRequired > 0)
                    {
                        totalPrice += (trolley.products.First(p => product.name == p.name).price ?? 0) * remainingRequired;
                        if (totalPrice >= lowestPrice)
                        {
                            invalidTotal = true;
                            break;
                        }
                    }
                }

                if (invalidTotal)
                {
                    continue;
                }
                lowestPrice = totalPrice;
            }
            return Task.FromResult(lowestPrice ?? 0);
        }

        private static List<List<T>> GetAllCombos<T>(List<T> list)
        {
            var comboCount = (int)Math.Pow(2, list.Count) - 1;
            List<List<T>> result = new List<List<T>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                result.Add(new List<T>());
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result;
        }
    }
}
