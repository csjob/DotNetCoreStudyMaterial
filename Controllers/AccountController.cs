using DotNetCoreWebAPI.Model;
using DotNetCoreWebAPI.Model.Dto;
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
            if (model.UserName == "admin" && model.Password == "admin")
            {
                var token = JwtHelper.GenerateJwtToken(model.UserName, UserRoles.Admin, _configuration);
                return Ok(new { token });
            }
            else if (model.UserName == "user" && model.Password == "user")
            {
                var token = JwtHelper.GenerateJwtToken(model.UserName, UserRoles.User, _configuration);
                return Ok(new { token });
            }else if (model.UserName == "manager" && model.Password == "manager")
            {
                var token = JwtHelper.GenerateJwtToken(model.UserName, "Department", "Management", _configuration);
                return Ok(new { token });
            }
                return Unauthorized();
        }

        [HttpGet("StoreCookie")]
        public IActionResult StoreCookie()
        {
            Response.Cookies.Append("userToken", "abc123", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            });
            return Ok();
        }

        [HttpGet("GetCookie")]
        public IActionResult GetCookie()
        {
            string? token = Request.Cookies["userToken"];
            return Ok(token);
        }

        [HttpGet("StoreSession")]
        public IActionResult StoreSession()
        {
            HttpContext.Session.SetString("username", "JobCS");
            return Ok();
        }

        [HttpGet("GetSession")]
        public IActionResult GetSession()
        {
            var user = HttpContext.Session.GetString("username");
            return Ok(user);
        }

        //[HttpPost("Register")]
        //public IActionResult Register(UserRegisterDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);  
        //    }
        //    return Ok("User is valid");
        //}
    }
}
