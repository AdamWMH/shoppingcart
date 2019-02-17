using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WooliesXAPI.Interfaces;
using WooliesXAPI.Models;

namespace WooliesXAPI.DataAccess
{
    public class ProductRepository : IProductRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IConfiguration Configuration { get; }

        public ProductRepository(IConfiguration injectedConfiguration)
        {
            this.Configuration = injectedConfiguration;
        }

        public async Task<List<Product>> GetProducts()
        {
            using (var client = new HttpClient())
            {
                var clientCodeUrl = $"{Configuration.GetSection("AppConfiguration")["test_api_url"]}/products?token={Configuration.GetSection("AppConfiguration")["test_api_key"]}";
                var response = await client.GetAsync(clientCodeUrl);
                var result = await response.Content.ReadAsStringAsync();

                var products = JsonConvert.DeserializeObject<List<Product>>(result);
                return products;
            }
        }

        public async Task<List<Customer>> GetShopperHistory()
        {
            using (var client = new HttpClient())
            {
                var clientCodeUrl = $"{Configuration.GetSection("AppConfiguration")["test_api_url"]}/shopperHistory?token={Configuration.GetSection("AppConfiguration")["test_api_key"]}";
                var response = await client.GetAsync(clientCodeUrl);
                var result = await response.Content.ReadAsStringAsync();

                var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
                return customers;
            }
        }

        public async Task<decimal> GetTrolleyTotal(Trolley trolley)
        {
            using (var client = new HttpClient())
            {
                var clientCodeUrl = $"{Configuration.GetSection("AppConfiguration")["test_api_url"]}/trolleyCalculator?token={Configuration.GetSection("AppConfiguration")["test_api_key"]}";
                var content = JsonConvert.SerializeObject(trolley);
                var response = await client.PostAsJsonAsync(clientCodeUrl, trolley);
                var result = await response.Content.ReadAsStringAsync();

                var total = JsonConvert.DeserializeObject<decimal>(result);
                return total;
            }
        }
    }
}
