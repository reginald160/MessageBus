using IzukaBus.Core.Crypto;
using IzukaBus.Data;
using IzukaBus.DTO;
using IzukaBus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IzukaBus.Controllers
{
    // Attribute to define the route and controller type for authentication
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger; // Logger for the controller
        private readonly ApplicationDbContext _context;    // Database context for accessing user data
        private readonly IConfiguration _configuration;    // Configuration to retrieve settings like key for JWT token

        // Constructor to inject the logger, context, and configuration dependencies
        public AuthController(ILogger<AuthController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger; // Assign the logger dependency
            _context = context; // Assign the database context
            _configuration = configuration; // Assign the configuration service
        }

        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDTO client)
        {
            if (ModelState.IsValid)
            {
                bool isExistingUserm = _context.Users.
                    Any(x => x.UserName.ToLower() == client.Email.ToLower());
                if (isExistingUserm)
                {
                    return BadRequest("User Exist");
                }
                var newUser = new Users
                {
                    Id = Guid.NewGuid(),
                    UserName = client.Email,
                    Password = PasswordHasher.HashPassword(client.Password),
                    IsActive = true

                };
                //e68cd019-32f7-493a-811a-6d929874878a
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return Ok(newUser.Id);
            }
            return BadRequest("Invalid request");
        }

        // API endpoint to authenticate the client using ClientId and SecretKey
        [HttpPost("Authenticate")]
        public async Task<IActionResult> AuthenticateClient([FromBody] TokenDTO dto)
        {
            try
            {
                // Log the start of the authentication attempt with the provided ClientId
                _logger.LogInformation("Attempting to authenticate client with ClientId: {ClientId}", dto.ClientId);

                // Fetch the client from the database by matching ClientId or UserId

                var client =  _context.Users
                    .FirstOrDefault(x => dto.ClientId == x.Id.ToString());

                // If client doesn't exist, return a 400 Bad Request response with an error message
                if (client == null)
                {
                    _logger.LogWarning("Authentication failed for ClientId: {ClientId}. Client does not exist.", dto.ClientId);
                    return BadRequest("Client does not exist");
                }

                // Verify the provided SecretKey against the stored password hash
                bool isPasswordValid = PasswordHasher.VerifyPassword(dto.SecretKey, client.Password);

                // If password is valid, generate a JWT token and return it
                if (isPasswordValid)
                {
                    var jwtKey = _configuration["JwtKey"]; // Retrieve the secret key for JWT generation
                    var token = JWTHelper.GenerateJwtTokenWithClaims(dto.ClientId, client.UserName, jwtKey);

                    _logger.LogInformation("Authentication successful for ClientId: {ClientId}", dto.ClientId);
                    return Ok(token);
                }
                else
                {
                    // Log failed authentication attempt due to invalid password
                    _logger.LogWarning("Invalid credentials for ClientId: {ClientId}. Authentication failed.", dto.ClientId);
                    return Unauthorized("Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic 500 internal server error response
                _logger.LogError(ex, "An error occurred during authentication for ClientId: {ClientId}", dto.ClientId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
