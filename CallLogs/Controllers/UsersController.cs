using AutoMapper;
using CallLogs.Data;
using CallLogs.Model;
using Dtos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace CallLogs.Controllers
{
	[EnableCors("appCors")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;

		public UsersController(ApplicationDbContext dbContext,IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
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

			var user = _mapper.Map<User>(userDto);
			
			_dbContext.Users.Add(user);
			_dbContext.SaveChanges();

			
			return Ok(userDto);
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
