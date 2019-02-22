using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WooliesXAPI.Interfaces;
using WooliesXAPI.Models;

namespace WooliesXAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WooliesXController : ControllerBase
    {
        private IProductService productService;

        public WooliesXController(IProductService injectedProductService)
        {
            productService = injectedProductService;
        }

        // GET api/user
        [HttpGet("user")]
        public ActionResult<User> GetUser()
        {
            return new User("Adam Walmsley", "ee29622c-a06d-42a6-8f54-4ff3004afba8");
        }

        [HttpGet("sort")]
        public async Task<ActionResult<List<Product>>> SortProducts([FromQuery] string sortOption)
        {
            var response = await productService.GetSortedProducts(sortOption);
            return response;
        }

        [HttpPost("trolleyTotal")]
        public async Task<ActionResult<decimal>> GetTrolleyTotal([FromBody] Trolley trolley)
        {
            //var trolley = JsonConvert.DeserializeObject<Trolley>(request);
            var response = await productService.GetTrolleyTotal(trolley);
            return response;
        }

        [HttpPost("v2/trolleyTotal")]
        public async Task<ActionResult<decimal>> CalculateTrolleyTotal([FromBody] Trolley trolley)
        {
            var response = await productService.CalculateTrolleyTotal(trolley);
            return response;
        }
    }
}
