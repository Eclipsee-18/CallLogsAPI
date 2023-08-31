using AutoMapper;
using CallLogs.Data;
using CallLogs.Dtos;
using CallLogs.Model;
using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CallLogs.Controllers
{
	[EnableCors("appCors")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;

		public readonly IConfiguration _configuration;

        public UsersController(ApplicationDbContext dbContext,IMapper mapper,IConfiguration configuration)
		{
			_dbContext = dbContext;
			_mapper = mapper;
            _configuration = configuration;
        }
		[HttpGet]
		public IActionResult GetUsers() 
		{
			var users = _dbContext.Users;

			var userDto=_mapper.Map<List<UserDto>>(users);

			return Ok(userDto);
			
		}

		[HttpGet("{userId}")]
		public IActionResult GetUserbyId(int userId)
		{
			var user = _dbContext.Users.FirstOrDefault(u=>u.UserId==userId);

			var userDto = _mapper.Map<UserDto>(user);

			return Ok(userDto);

		}

		[HttpPost]
		public IActionResult CreateAccount([FromBody] UserDto userDto)
		{
			
			userDto.PhoneNumber = GeneratePhoneNumber();
            userDto.Password = HashPassword(userDto.Password);

            var user = _mapper.Map<User>(userDto);

           

            _dbContext.Users.Add(user);
			_dbContext.SaveChanges();

			
			return Ok(userDto);
		}

		[HttpPost("login"), AllowAnonymous]
		public IActionResult Login(LoginRequest request)
		{
            var user = _dbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
			if (user == null)
			{
				return BadRequest("User not found");
			}

			if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
			{
				return BadRequest("Wrong password");
			}

			string token = CreateToken(user);
			return Ok(token);
        }

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.MobilePhone,user.PhoneNumber)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				_configuration.GetSection("AppSettings:Token").Value!));

			var cred=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: cred
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
			
		}

        private string HashPassword(string password)
        {
           
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12); 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        [HttpGet("{userId}/calllogs")]
		public IActionResult GetCallLogsByUserId(int userId)
		{
			var user = _dbContext.Users.Include(u => u.CallLogs).FirstOrDefault(u => u.UserId == userId);
			if (user == null)
			{
				return NotFound(); 
			}

			
			var callLogs = user.CallLogs;

			
			var callLogsDto = _mapper.Map<List<CalllogDto>>(callLogs);

			return Ok(callLogsDto);
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteUser(int id)
		{
			var user = _dbContext.Users.Find(id);
			if (user == null)
			{
				return NotFound(); 
			}

			_dbContext.Users.Remove(user);
			_dbContext.SaveChanges();

			return Ok(user);
		}

		private string GeneratePhoneNumber()
		{
			
			Random random = new Random();
			return random.Next(100_000_000, 1_000_000_000).ToString();
		}
	}
}
