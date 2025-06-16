using DotNetCoreWebAPI.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public ProductController(IMessageService messageService)
        {
            _messageService= messageService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(new[] {"Audi", "Benz", "BMW"});

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok($"Car {id}");

        [HttpPost]
        public IActionResult Create([FromBody] string model) => Ok($"{model} created.");

        [HttpGet("GetMessage")]
        public IActionResult GetMessage() => Ok(_messageService.GetMessage());
    }
}
