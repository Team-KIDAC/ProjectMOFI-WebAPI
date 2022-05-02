using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectMOFI_Server_WebAPI.DTOs;
using ProjectMOFI_Server_WebAPI.Models;
using ProjectMOFI_Server_WebAPI.MongoDB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ProjectMOFI_Server_WebAPI.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        //The API Controller to handle all the authentication and authorization operations.

        IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        MongoConnection _connection;

        public AuthController(IWebHostEnvironment env, IConfiguration config) {
            _config = config;
            _webHostEnvironment = env;
            _connection = new MongoConnection(config);
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult> RegisterUser(LoginUserDto loginUserDto) {
            //The HTTP method to register a new user by the username and the password.

            CreatePasswordHash(loginUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            try {
                _connection.InsertLoginUserHash(new LoginUser() {
                    Username = loginUserDto.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                });

                return Created(loginUserDto.Username, "Successfully registered the user.");
            }
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<string>> LogInUser(LoginUserDto loginUserDto) {
            //The HTTP method to log an user in using the username and the password hash which is generated using the entered password.

            try {
                LoginUser loginUser = _connection.LoadLoginUserByUsername(loginUserDto.Username);

                if (!VerifyPasswordHash(loginUserDto.Password, loginUser.PasswordHash, loginUser.PasswordSalt)) {
                    return BadRequest("Wrong password!");
                }

                string token = CreateToken(loginUser);
                return Ok(token);
            }
            catch(ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("LoggedInUser"), Authorize(Roles = "Student")]
        public async Task<ActionResult<string>> GetLoggedInUser() {
            //The HTTP method to return the name of the authorized owner(user) of the JWT token.

            string? userName = User?.Identity?.Name;
            return Ok(userName);
        }

        private string CreateToken(LoginUser user) {
            //A helper method to create a new JWT token.

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Student")
            };
            string secretKey = _config["SymmetricSecurityKeyToken"];
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            //A helper method to compare the stored password hash with the generated password hash using the entered password.

            using (HMACSHA512 hmac = new HMACSHA512(passwordSalt)) {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            //A helper method to generate a new pawwsord hash using the entered password.

            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
