﻿using Common.Authentication;
using Common.DTO.Account;
using Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Utils;

namespace SkillWars.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService, ILoginService loginService)
        {
            _loginService = loginService;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        public async Task<IActionResult> Register([FromBody]RegistrationDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null)
            {
                return BadRequest();
            }

            var response = await _accountService.Register(request);

            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok();
        }



        [AllowAnonymous]
        [HttpPost("SteamRegister")]
        public async Task<IActionResult>SteamRegister([FromBody]SteamRegistrationDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.SteamRegister(request);

            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok();
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Convert.ToInt32(User.GetUserId());
            var response = await _accountService.ChangePassword(request, userId);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpGet("Confirm/{token}")]
        public async Task<IActionResult> ConfirmEmail([FromRoute]string token)
        {
            var response = await _accountService.ConfirmEmail(token);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }

            //await _socketServer.ConfirmNotify(new Notification<UserDTO>
            //{
            //    Action = NotificationActions.ConfirmEmail,
            //    Data = response.Data
            //});

            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpGet("Users/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute]int id)
        {
            var response = await _accountService.GetUserById(id);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {            
            var response = await _accountService.GetUserById(Convert.ToInt32(User.GetUserId()));
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpDelete("Users/{id}")]
        public async Task<IActionResult> RemoveUser([FromRoute]int id)
        {
            var response = await _accountService.RemoveUser(id);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }

            //await _socketServer.ConfirmNotify(new Notification<UserDTO>
            //{
            //    Action = NotificationActions.ConfirmEmail,
            //    Data = response.Data
            //});

            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpPost("Token")]
        public async Task<IActionResult> Token([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await _loginService.GetIdentity(request.Login, request.Password);
            if (identity.Error != null)
            {
                return StatusCode(identity.Error.ErrorCode, identity.Error);
            }

            return Ok(await _loginService.GetToken(identity.Data));
        }

        [AllowAnonymous]
        [HttpPost("Token/{steamId}")]
        public async Task<IActionResult> SteamToken([FromRoute]string steamId)
        {
            var identity = await _loginService.GetIdentity(steamId);
            if (identity.Error != null)
            {
                return StatusCode(identity.Error.ErrorCode, identity.Error);
            }
            
            return Ok(await _loginService.GetToken(identity.Data));
        }

        [AllowAnonymous]
        [HttpGet("Users")]
        public async Task<List<UserDTO>> GetUsers()
        {      
            return await _accountService.GetUsers();
        }

        [AllowAnonymous]
        [HttpPut("ForgotPassword")]
        public async Task<IActionResult> RestorePassword([FromBody] string requestEmail)
        {
            var response = await _accountService.RestorePassword(requestEmail);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }

        [AllowAnonymous]
        [HttpPut("ForgotPasswordConfirm")]
        public async Task<IActionResult> RestorePasswordConfirm([FromBody] RestorePasswordRequest request)
        {
            var response = await _accountService.RestorePasswordConfirm(request.Token, request.NewPassword);
            if (response.Error != null)
            {
                return StatusCode(response.Error.ErrorCode, response.Error);
            }
            return Ok(response.Data);
        }
    }
}
