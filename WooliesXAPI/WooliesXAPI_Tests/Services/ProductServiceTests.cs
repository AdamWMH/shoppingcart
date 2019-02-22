using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using WooliesXAPI.Interfaces;
using WooliesXAPI.Models;
using WooliesXAPI.Services;

namespace WooliesXAPI_Tests.Services
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> mockProductRepository;
        [SetUp]
        public void Setup()
        {
            mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(x => x.GetProducts()).Returns(() => Task.FromResult(new List<Product> {new Product {
                        name = "Test Product A",
                        price = 99.99M,
                        quantity = 0
                    },
                    new Product{
                        name = "Test Product B",
                        price = 101.99M,
                        quantity = 0
                    },
                    new Product{
                        name = "Test Product C",
                        price = 10.99M,
                        quantity = 0
                    },
                    new Product{
                        name = "Test Product D",
                        price = 5M,
                        quantity = 0
                    },
                    new Product{
                        name = "Test Product F",
                        price = 999999999999M,
                        quantity = 0
                    }}));
            mockProductRepository.Setup(x => x.GetShopperHistory()).Returns(() => Task.FromResult(new List<Customer> {new Customer{
                    customerId = 123,
                    products = new List<Product> {
                        new Product{
                            name =  "Test Product A",
                            price =  99.99M,
                            quantity =  3
                        },
                        new Product{
                            name =  "Test Product B",
                            price =  101.99M,
                            quantity =  1
                        },
                        new Product{
                            name =  "Test Product F",
                            price =  999999999999M,
                            quantity =  1
                        }
                    }
                },
                new Customer{
                    customerId = 23,
                    products = new List<Product> {
                        new Product{
                            name =  "Test Product A",
                            price =  99.99M,
                            quantity =  2
                        },
                        new Product{
                            name =  "Test Product B",
                            price =  101.99M,
                            quantity =  3
                        },
                        new Product{
                            name =  "Test Product F",
                            price =  999999999999M,
                            quantity =  1
                        }
                    }
                },
                new Customer{
                    customerId = 23,
                    products = new List<Product> {
                        new Product{
                            name =  "Test Product C",
                            price =  10.99M,
                            quantity =  2
                        },
                        new Product{
                            name =  "Test Product F",
                            price =  999999999999M,
                            quantity =  2
                        }
                    }
                },
                new Customer{
                    customerId = 23,
                    products = new List<Product> {
                        new Product{
                            name =  "Test Product A",
                            price =  99.99M,
                            quantity =  1
                        },
                        new Product{
                            name =  "Test Product B",
                            price =  101.99M,
                            quantity =  1
                        },
                        new Product{
                            name =  "Test Product C",
                            price =  10.99M,
                            quantity =  1
                        }
                    }
                }}));
            mockProductRepository.Setup(x => x.GetTrolleyTotal(new Trolley())).Returns(() => Task.FromResult(14M));
            //mockProductRepository.Setup(x => x.ValidateCurrencyCode("ANZ")).Returns(() => Task.FromResult(false));
            //mockProductRepository.Setup(x => x.GetExchangeRate("AUD", "USD")).Returns(() => Task.FromResult(new ExchangeRate { Rate = 0.75M, Timestamp = DateTime.Now }));
            //mockProductRepository.Setup(x => x.GetExchangeRate("USD", "AUD")).Returns(() => null);
        }

        [TestCase("Low", "[{\"name\": \"Test Product D\",\"price\": 5,\"quantity\": 0},{\"name\": \"Test Product C\",\"price\": 10.99,\"quantity\": 0},{\"name\": \"Test Product A\",\"price\": 99.99,\"quantity\": 0},{\"name\": \"Test Product B\",\"price\": 101.99,\"quantity\": 0},{\"name\": \"Test Product F\",\"price\": 999999999999,\"quantity\": 0}]")]
        [TestCase("High", "[{\"name\": \"Test Product F\",\"price\": 999999999999,\"quantity\": 0},{\"name\": \"Test Product B\",\"price\": 101.99,\"quantity\": 0},{\"name\": \"Test Product A\",\"price\": 99.99,\"quantity\": 0},{\"name\": \"Test Product C\",\"price\": 10.99,\"quantity\": 0},{\"name\": \"Test Product D\",\"price\": 5,\"quantity\": 0}]")]
        [TestCase("Ascending", "[{\"name\": \"Test Product A\",\"price\": 99.99,\"quantity\": 0},{\"name\": \"Test Product B\",\"price\": 101.99,\"quantity\": 0},{\"name\": \"Test Product C\",\"price\": 10.99,\"quantity\": 0},{\"name\": \"Test Product D\",\"price\": 5,\"quantity\": 0},{\"name\": \"Test Product F\",\"price\": 999999999999,\"quantity\": 0}]")]
        [TestCase("Descending", "[{\"name\": \"Test Product F\",\"price\": 999999999999,\"quantity\": 0},{\"name\": \"Test Product D\",\"price\": 5,\"quantity\": 0},{\"name\": \"Test Product C\",\"price\": 10.99,\"quantity\": 0},{\"name\": \"Test Product B\",\"price\": 101.99,\"quantity\": 0},{\"name\": \"Test Product A\",\"price\": 99.99,\"quantity\": 0}]")]
        [TestCase("Recommended", "[{\"name\": \"Test Product A\",\"price\": 99.99,\"quantity\": 0},{\"name\": \"Test Product B\",\"price\": 101.99,\"quantity\": 0},{\"name\": \"Test Product F\",\"price\": 999999999999,\"quantity\": 0},{\"name\": \"Test Product C\",\"price\": 10.99,\"quantity\": 0},{\"name\": \"Test Product D\",\"price\": 5,\"quantity\": 0}]")]
        public async Task TestProductSortOptions(string sortOption, string expectedResult)
        {
            var testRepo = new ProductService(mockProductRepository.Object);
            var sortedProducts = await testRepo.GetSortedProducts(sortOption);
            var list = JsonConvert.DeserializeObject<List<Product>>(expectedResult);
            Assert.AreEqual(sortedProducts, list);
        }

        [Test]
        public async Task TestGetTrolleyTotal()
        {
            var testRepo = new ProductService(mockProductRepository.Object);
            var trolleyTotal = await testRepo.GetTrolleyTotal(new Trolley());
            Assert.GreaterOrEqual(trolleyTotal, 14);
        }

        [TestCase(15, "{\"Products\":[{\"Name\":\"1\",\"Price\":2.0},{\"Name\":\"2\",\"Price\":5.0},{\"Name\":\"3\",\"Price\":3.0}],\"Specials\":[{\"Quantities\":[{\"Name\":\"1\",\"Quantity\":3},{\"Name\":\"2\",\"Quantity\":0},{\"Name\":\"3\",\"Quantity\":2}],\"Total\":5.0},{\"Quantities\":[{\"Name\":\"1\",\"Quantity\":2},{\"Name\":\"2\",\"Quantity\":2},{\"Name\":\"3\",\"Quantity\":0}],\"Total\":10.0}],\"Quantities\":[{\"Name\":\"1\",\"Quantity\":3},{\"Name\":\"2\",\"Quantity\":2},{\"Name\":\"3\",\"Quantity\":2}]}")]
        public async Task TestCalculateTrolleyTotal(decimal expectedResult, string trolleyJson)
        {
            var trolley = JsonConvert.DeserializeObject<Trolley>(trolleyJson);
            var testRepo = new ProductService(mockProductRepository.Object);
            var trolleyTotal = await testRepo.CalculateTrolleyTotal(trolley);
            Assert.AreEqual(trolleyTotal, expectedResult);
        }

    }
}