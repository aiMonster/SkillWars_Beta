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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLobbie([FromBody]LobbieDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _lobbieService.CreateLobbieAsync(request));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetLobbies()
        {
            return Ok(await _lobbieService.GetLobbiesAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}/Players")]
        public async Task<IActionResult> GetPlayers([FromRoute]int id)
        {
            var response = await _lobbieService.GetPlayersAsync(id);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("Teams/Participate")]
        public async Task<IActionResult> ParticipateLobbie([FromBody] ParticipateLobbieRequest request)
        {
            var response = await _lobbieService.ParticipateTeam(Convert.ToInt32(User.GetUserId()), request);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [Authorize]
        [HttpPut("{lobbieId}/Leave")]
        public async Task<IActionResult> LeaveLobbie([FromRoute] int lobbieId)
        {
            var response = await _lobbieService.LeaveTeam(Convert.ToInt32(User.GetUserId()), lobbieId);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpDelete("{lobbieId}")]
        public async Task<IActionResult> RemoveLobbie([FromRoute] int lobbieId)
        {
            var response = await _lobbieService.RemoveLobbie(lobbieId);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }
    }
}
