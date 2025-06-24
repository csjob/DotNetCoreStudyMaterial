using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DotNetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllRolesController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/data")]
        public IActionResult GetAdminData() => Ok("Hello Admin!");

        [Authorize(Roles = "User,Admin")]
        [HttpGet("profile")]
        public IActionResult GetUserData() => Ok("Hello User!");

        [Authorize(Policy = "MustBeManager")]
        [HttpGet("manager/section")]
        public IActionResult ManagerSection() => Ok("Welcome, Manager!");

    }
}
