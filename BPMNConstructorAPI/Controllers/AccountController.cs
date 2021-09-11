using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BPMNConstructorAPI.Models;
using BPMNConstructorAPI.Models.Input;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BPMNConstructorAPI.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] UserInput request)
        {
            var identity = await GetIdentity(request.UserName, request.Password);
            if (identity == null)
            {
                return BadRequest(new
                {
                    errorText = "Invalid username or password."
                });
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                accessToken = encodedJwt,
                role = identity.Claims.First(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value
            };
            return new JsonResult(response);
        }
        
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(UserInput model)
        {
            User user = new User()
            {
                Email = model.Email,
                UserName = model.UserName
            };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    var token = await SignIn(model);
                    return token;
                }

                return BadRequest(result.Errors);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        private async Task<ClaimsIdentity> GetIdentity(string userName, string password)
        {
            User user =  _userManager.Users.FirstOrDefault(u=>u.UserName == userName);
            var result = await _userManager.CheckPasswordAsync(user, password);
            if (user != null && result)
            {
                var role = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role.First()),
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims,
                        "Token",
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}