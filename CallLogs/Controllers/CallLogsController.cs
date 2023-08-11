using AutoMapper;
using CallLogs.Data;
using Dtos;
using CallLogs.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace CallLogs.Controllers
{
	[EnableCors("appCors")]
	[Route("api/[controller]")]
	[ApiController]
	public class CallLogsController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IMapper _mapper;

		public CallLogsController(ApplicationDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		[HttpPost]
		public IActionResult CreateCallLog(int userId,  CalllogDto callLogDto, int callerId)
		{
			var user = _dbContext.Users.Find(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var callerUser = _dbContext.Users.Find(callerId);
			if (callerUser == null)
			{
				return NotFound("Caller not found.");
			}

			callLogDto = new CalllogDto
			{
				Outgoing = true,
				Incoming = false,
				Missed = false,
				CallerId = callerId
			};
			var callLog = _mapper.Map<Calllog>(callLogDto);
			callLog.UserId = userId;
			callLog.CallerId = callerId;

			var incomingCallLog = new Calllog
			{
				UserId = callerId,
				CallerId = userId,
				Outgoing = false,
				Incoming = true,
				Missed = false
			};

			_dbContext.CallLogs.Add(callLog);
			_dbContext.CallLogs.Add(incomingCallLog);
			_dbContext.SaveChanges();

			return Ok(callLogDto);
		}

	}
}
