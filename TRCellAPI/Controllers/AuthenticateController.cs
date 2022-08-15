using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TRCellAPI.Controllers{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthenticateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("JwtAuth")]
        public async Task<IActionResult> Login([FromBody] JWTLoginModel model)
        {
            string userName = model.Username;
            string password = model.Password;

            bool isSuccess = false;
            if (userName == "webUser")
            {
                if (password == _configuration["JWT:webUser"])
                {
                    isSuccess = true;
                }
            }
            else if (userName == "aiUser")
            {
                if (password == _configuration["JWT:aiUser"])
                {
                    isSuccess = true;
                }
            }

            if (isSuccess)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                    new Claim(JwtRegisteredClaimNames.Email, model.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var token = new JwtSecurityToken(
                  issuer: _configuration["JWT:ValidIssuer"],
                  audience: _configuration["JWT:ValidIssuer"],
                  claims,
                  expires: DateTime.Now.AddHours(4),
                  signingCredentials: credentials
                  );

                var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(encodetoken);
            }

            return Unauthorized();
        }
    }
}
