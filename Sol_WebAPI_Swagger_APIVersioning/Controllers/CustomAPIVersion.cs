using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sol_WebAPI_Swagger_APIVersioning.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomAPIVersion : ControllerBase
    {

        [HttpGet("GetTeam")]
        [MapToApiVersion("1.0")]
        public IActionResult GetV1()
        {
            return Ok("V1 Get to be implemented");
        }


        [HttpGet("GetTeam")]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Ok("V2 Get to be implemented");
        }


        [HttpPost]
        public IActionResult Post(Object team)
        {
            return Ok("V1 Post to be implemented");
        }
    }
}
