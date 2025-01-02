using Hangfire;
using IzukaBus.BackgroundJobs;
using IzukaBus.Core.Crypto;
using IzukaBus.Data;
using IzukaBus.DTO;
using IzukaBus.EventHub;
using IzukaBus.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IzukaBus.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EventController : ControllerBase
	{
		

		private readonly ILogger<EventController> _logger;
		private readonly IHubContext<MessageHub> _hubContext;
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly ApplicationDbContext _dbContext;
		private readonly TimerManager _timer;
		public EventController(ILogger<EventController> logger, IHubContext<MessageHub> hubContext, IBackgroundJobClient backgroundJobClient, ApplicationDbContext dbContext, TimerManager timer)
		{
			_logger = logger;
			_hubContext=hubContext;
			_backgroundJobClient=backgroundJobClient;
			_dbContext=dbContext;
			_timer=timer;
		}



		[HttpGet("LiveChat")]
		public IActionResult Get()
		{
			if (!_timer.IsTimerStarted)
				_timer.PrepareTimer(() => _hubContext.Clients.All.SendAsync("TransferChartData", DataManager.GetData()));
			return Ok(new { Message = "Request Completed" });
		}


	}
}
