using Hangfire;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using IzukaBus.EventHub;
using Microsoft.AspNetCore.SignalR;
using static IzukaBus.EventHub.MessageHub;

namespace IzukaBus.BackgroundJobs
{
	public class SheduleService
	{
		private readonly IHubContext<MessageHub> _hubContext;

		public SheduleService(IHubContext<MessageHub> hubContext)
		{
			_hubContext=hubContext;
		}
		public async Task SendNotificationAsync(string route, Notification notification)
		{
			await _hubContext.Clients.All.SendAsync(route, notification);
		}
	}
}
