using Common.DTO.Lobbie;
using Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SkillWars.Controllers
{
    [Route("api/[controller]")]
    public class LobbiesController : Controller
    {
        private readonly ILobbieService _lobbieService;

        public LobbiesController(ILobbieService lobbieService)
        {
            _lobbieService = lobbieService;
        }

        [AllowAnonymous]
        //[Authorize]
        [HttpPost]
        public async Task<LobbieInfo> CreateLobbie([FromBody]LobbieDTO request)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var userId = Convert.ToInt32(User.GetUserId());
            return await _lobbieService.CreateLobbie(request);
            //if (response.Error != null)
            //{
            //    return StatusCode(response.Error.ErrorCode, response.Error);
            //}
            //return response;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<LobbieInfo>> GetLobbies()
        {
            return await _lobbieService.GetLobbies();
        }

        [AllowAnonymous]        
        [HttpGet("{id}/Players")]
        public async Task<IActionResult> GetPlayers([FromRoute]int id)
        {
            

            //var userId = Convert.ToInt32(User.GetUserId());
            var response =  await _lobbieService.GetPlayers(id);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("Teams/{teamId}/Participate")]
        public async Task<IActionResult> ParticipateLobbie([FromRoute] int teamId, string password = "")
        {
            var response = await _lobbieService.ParticipateTeam(Convert.ToInt32(User.GetUserId()), teamId, password);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("Teams/{teamId}/Leave")]
        public async Task<IActionResult> LeaveLobbie([FromRoute] int teamId)
        {
            var response = await _lobbieService.LeaveTeam(Convert.ToInt32(User.GetUserId()), teamId);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }
    }
}
