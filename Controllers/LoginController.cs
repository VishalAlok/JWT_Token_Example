using JWT_Token_Example.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JWT_Token_Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {

            _config = config;

        }
        private Users AuthenticateUser(Users user)
        {
            Users _user = null;
            if(user.Username == "admin" && user.Password == "12345")
            {
                _user = new Users { Username = "Vishal Alok" };
            }
            return _user;
        }

        private string GenerateToken(Users user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["jwt:Issuer"], _config["jwt:Audience"],null,
                expires: DateTime.Now.AddMinutes(1),signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Users user)
        {
            IActionResult response = Unauthorized();
            var _user = AuthenticateUser(user);
            if(_user != null)
            { 
                var token = GenerateToken(_user);
                response = Ok(new { token = token });
            }
            return response;
        }
    }
}
