using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TVMenukaart.DTO;
using TVMenukaart.Interfaces;
using TVMenukaart.Models;

namespace TVMenukaart.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return Conflict("Gebruikersnaam is al in gebruik");
            }

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                Email = registerDto.Username.ToLower()
            };

            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await SetRefreshToken(user);

            return Ok(CreateUserObject(user));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x =>
                x.NormalizedUserName == loginDto.Username.ToUpper()
            );

            if (user == null)
            {
                return Problem(type: "Not Found", title: "User not found",
                    statusCode: StatusCodes.Status404NotFound);
                // return Unauthorized("Gebruikersnaam is ongeldig");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Problem(type: "Not Found", title: "User/password is invalid",
                    statusCode: StatusCodes.Status404NotFound);
            }

            await SetRefreshToken(user);
            return Ok(CreateUserObject(user));
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users.Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null)
            {
                return Unauthorized();
            }

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
            {
                return Unauthorized();
            }

            if (oldToken != null)
            {
                oldToken.Revoked = DateTime.UtcNow;
            }

            return Ok(CreateUserObject(user));
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                Token = _tokenService.CreateToken(user),
                Email = user.Email,
                Id = user.Id,
                Username = user.UserName
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }
}
