using DotNetCoreWebAPI.DI;
using DotNetCoreWebAPI.Model;
using DotNetCoreWebAPI.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly AppDbContext _context;

        public ProductController(IMessageService messageService, AppDbContext context)
        {
            _messageService= messageService;
            _context= context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(new[] {"Audi", "Benz", "BMW"});

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok($"Car {id}");

        [HttpPost]
        public IActionResult Create([FromBody] string model) => Ok($"{model} created.");

        [HttpGet("GetMessage")]
        public IActionResult GetMessage() => Ok(_messageService.GetMessage());

        [HttpPost("AddProduct")]
        public IActionResult AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts() => Ok(_context.Products.ToList());
    }
}
