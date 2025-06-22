using DotNetCoreWebAPI.Model;
using DotNetCoreWebAPI.Model.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin model)
        {
            if(model.UserName == "admin" && model.Password == "admin")
            {
                var token = JwtHelper.GenerateJwtToken(model.UserName, _configuration);
                return Ok(new {token});
            }
            return Unauthorized();
        }
    }
}
