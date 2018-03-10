using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSocketLayer;

namespace SkillWars.Controllers
{
    [Route("api/OnlyAuthorized")]
    [Authorize]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Nice Job", "you are authorized" };
        }

        [AllowAnonymous]
        [HttpPut("ForgotPassword")]
        public async Task<IActionResult> RestorePassword([FromBody] string requestEmail)
        {            
            return Ok(requestEmail);
        }

        // GET api/values/5       
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            return "Nice Job, you are authorized";         
        }   

        [AllowAnonymous]
        [HttpGet("log")]
        public async Task<List<string>> Logging()
        {
            return HardLogger.Logs;
        }
    
    }
}
