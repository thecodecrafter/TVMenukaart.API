using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteMenu.Data;
using RemoteMenu.DTO;
using RemoteMenu.Interfaces;
using RemoteMenu.Models;

namespace RemoteMenu.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly RemoteMenuContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;

        public AuthController(RemoteMenuContext context, UserManager<AppUser> userManager, ILogger<AuthController> logger, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet("device-code")]
        [ProducesResponseType(typeof(DeviceCode), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GenerateDeviceCode()
        {
            var generateDeviceCode = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
            var pollingToken = Guid.NewGuid().ToString();

            var deviceCode = new DeviceCode()
            {
                Code = generateDeviceCode,
                PollingToken = pollingToken,
                ExpirationInSeconds = 300,
                TimeStamp = DateTime.UtcNow
            };

            _context.DeviceCodes.Add(deviceCode);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New device code generated and successfully saved {deviceCode.Code}");

            return Ok(deviceCode);
        }

        [HttpPost("poll")]
        [ProducesResponseType(typeof(PollingResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PollLoginStatus([FromBody] PollingRequest request)
        {
            var user = await CheckUserAuthenticationStatus(request.PollingToken);

            if (user != null)
            {
                return Ok(new PollingResponse()
                {
                    IsAuthenticated = true,
                    User = new UserDto
                    {
                        Email = user.Email,
                        Id = user.Id,
                        Username = user.UserName,
                        Token = _tokenService.CreateToken(user)
                    }
                });
            }

            return Ok(new { isAuthenticated = false });
        }

        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            var deviceAuth = await _context.DeviceCodes.FirstOrDefaultAsync(d => d.Code == request.Code);

            if (deviceAuth == null || deviceAuth.TimeStamp.AddSeconds(deviceAuth.ExpirationInSeconds) < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired code." });
            }

            deviceAuth.UserId = user.Id.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { message = "Device successfully linked!" });
        }

        private async Task<AppUser> CheckUserAuthenticationStatus(string pollingToken)
        {
            var deviceAuth = await _context.DeviceCodes.FirstOrDefaultAsync(d => d.PollingToken == pollingToken);

            if (deviceAuth == null)
            {
                throw new ArgumentException("Invalid polling token");
            }

            if (deviceAuth.TimeStamp.AddSeconds(deviceAuth.ExpirationInSeconds) < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Polling token has expired");
            }

            if (!string.IsNullOrEmpty(deviceAuth.UserId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == deviceAuth.UserId);
                return user;
            }

            return null;
        }
    }

    public class PollingRequest
    {
        public string PollingToken { get; set; }
    }

    public class PollingResponse
    {
        public UserDto User { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class VerifyCodeRequest
    {
        public string Code { get; set; }
    }
}
