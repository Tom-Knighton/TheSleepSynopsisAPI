using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService auth, ITokenService tokens)
        {
            _authService = auth;
            _tokenService = tokens;
        }

        [HttpPost("Authenticate")]
        [Produces(typeof(User))]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthRequest auth, bool needsTokens = true)
        {
            try
            {
                return Ok(await _authService.Authenticate(auth, needsTokens));
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.description);
            }
        }

        [HttpPost("Refresh")]
        [Produces(typeof(UserAuthenticationTokens))]
        public async Task<IActionResult> RefreshTokens([FromBody] RefreshRequestDTO dto)
        {
            UserAuthenticationTokens tokens = await _tokenService.RefreshTokensForUser(dto.UserUUID, dto.RefreshToken);
            if (tokens == null)
            {
                return BadRequest("Invalid refresh token");
            }

            return Ok(tokens);
        }

        [HttpPost("Register")]
        [Produces(typeof(User))]
        public async Task<IActionResult> RegisterUser([FromBody] NewUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid DTO");

            try
            {
                User? user = await _authService.CreateUser(dto);
                return Ok(user);
            }
            catch (RegistrationException ex)
            {
                return BadRequest(ex.description);
            }
        }
    }
}

