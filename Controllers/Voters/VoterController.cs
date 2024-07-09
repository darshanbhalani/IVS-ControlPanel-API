using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IVS_API.Controllers.Voters
{
    [Route("[controller]")]
    [ApiController]
    public class VoterController : ControllerBase
    {
        [HttpGet("GetVoterByEpic")]
        public IActionResult GetVoterByEpic() {
            return Ok();
        }


    }
}
