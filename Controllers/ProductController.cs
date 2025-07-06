using AutoMapper;
using DotNetCoreWebAPI.DI;
using DotNetCoreWebAPI.Model;
using DotNetCoreWebAPI.Model.Db;
using DotNetCoreWebAPI.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetCoreWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region For Practicing day 1 to day 3 
        //private readonly IMessageService _messageService;
        //private readonly AppDbContext _context;

        //public ProductController(IMessageService messageService, AppDbContext context)
        //{
        //    _messageService= messageService;
        //    _context= context;
        //}

        //[HttpGet]
        //public IActionResult GetAll() => Ok(new[] {"Audi", "Benz", "BMW"});

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id) => Ok($"Car {id}");

        //[HttpPost]
        //public IActionResult Create([FromBody] string model) => Ok($"{model} created.");

        //[HttpGet("GetMessage")]
        //public IActionResult GetMessage() => Ok(_messageService.GetMessage());

        //[HttpPost("AddProduct")]
        //public IActionResult AddProduct(Product product)
        //{
        //    _context.Products.Add(product);
        //    _context.SaveChanges();
        //    return Ok(product);
        //}

        //[HttpGet("GetProducts")]
        //public IActionResult GetProducts() => Ok(_context.Products.ToList());
        #endregion

        #region for EF Crud Operation with MySQL
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        public ProductController(AppDbContext dbContext, IMapper mapper, IMemoryCache cache) { 
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => 
            Ok(await _dbContext.Products.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var existing = await _dbContext.Products.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = product.Name;
            existing.Price= product.Price;
            await _dbContext.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if(product == null) return NotFound();

            _dbContext.Products.Remove(product);    
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Different Actions using LINQ

        [HttpGet("GetExpensiveProducts")]
        public async Task<IActionResult> GetExpensiveProducts()
        {
            var expensiveProduct = await _dbContext.Products
                .Where(p => p.Price > 500)
                .ToListAsync();
            return Ok(expensiveProduct);    
        }

        [HttpGet("SortProducts")]
        public async Task<IActionResult> SortProducts()
        {
            var sorted = await _dbContext.Products
               .OrderBy(p => p.Price)
               .ToListAsync();
            return Ok(sorted);    
        }

        [HttpGet("GetProductNames")]
        public async Task<IActionResult> GetProductNames()
        {
            var names = await _dbContext.Products
                 .Select(p => p.Name)
                 .ToListAsync();
            return Ok(names);    
        }

        [HttpGet("GetProductNames/{id}")]
        public async Task<IActionResult> GetByProductId(int id)
        {
            var single = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            return Ok(single);    
        }

        [HttpGet("GetProductsDto")]
        public async Task<IActionResult> GetProductsDto()
        {
            var products = await _dbContext.Products.ToListAsync();
            var dtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(dtos);
        }


        #endregion

        #region Cache Concept
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            if(!_cache.TryGetValue("all-products", out List<Product> products))
            {
                products = await _dbContext.Products.ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set("all-products", products, cacheOptions);
            }
            return Ok(products);
        }

        [HttpGet("get-time")]
        [ResponseCache(Duration = 60)]
        public IActionResult GetTime()
        {
            return Ok(DateTime.Now);
        }

        [HttpPost("get-time-post")] // this cache does not work. because it is post request.
        [ResponseCache(Duration = 60)]
        public IActionResult GetTimePost()
        {
            return Ok(DateTime.Now); 
        }

        #endregion 
    }
}
