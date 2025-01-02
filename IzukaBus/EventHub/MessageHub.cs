using Hangfire;
using IzukaBus.BackgroundJobs;
using IzukaBus.Data;
using IzukaBus.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IzukaBus.EventHub
{
    /// <summary>
    /// The MessageHub class is a SignalR Hub that handles real-time communication with clients.
    /// It allows broadcasting chart data and sending notifications asynchronously.
    /// </summary>
    [AllowAnonymous] // Allows connections without requiring authorization
    public class MessageHub : Hub
    {
        // Private members to hold services used by the Hub
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHubContext<MessageHub> _hubContext;

        // Record to define a Notification structure
        public record Notification(string Text, DateTime Date, string topic = "");

        // Static dictionary to track topic subscriptions (this could be useful in some scenarios)
        private static readonly Dictionary<string, HashSet<string>> _topicSubscriptions = new();

        /// <summary>
        /// Constructor to initialize dependencies through Dependency Injection.
        /// </summary>
        /// <param name="backgroundJobClient">Hangfire client to manage background jobs.</param>
        /// <param name="dbContext">Database context for checking user validity.</param>
        /// <param name="hubContext">SignalR context to send messages to clients.</param>
        public MessageHub(IBackgroundJobClient backgroundJobClient, ApplicationDbContext dbContext, IHubContext<MessageHub> hubContext)
        {
            _backgroundJobClient = backgroundJobClient;
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Broadcast chart data to all connected clients.
        /// </summary>
        /// <param name="data">List of chart data to send.</param>
        public async Task BroadcastChartData(List<ChartModel> data)
        {
            // Send the chart data to all clients connected to this Hub
            await Clients.All.SendAsync("broadcastchartdata", data);
        }

        /// <summary>
        /// Publish a notification that will be handled by a background job.
        /// </summary>
        /// <param name="notification">The notification object containing the message to send.</param>
        public void PublishNotification(Notification notification)
        {
            // Retrieve clientId from the current user's claims
            var clientId = Context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Generate a unique method name for the background job using the topic and clientId
            var methodName = notification.topic + "-" + clientId;

            // Enqueue the background job to send the notification
            _backgroundJobClient.Enqueue(() => SendNotificationInBackground(methodName, notification));
        }

        /// <summary>
        /// This method is executed as a background job to send notifications asynchronously.
        /// </summary>
        /// <param name="route">The route to which the notification should be sent.</param>
        /// <param name="notification">The notification data to be sent.</param>
        public async Task SendNotificationInBackground(string route, Notification notification)
        {
            // Initialize the service to handle notification sending
            var backgroundService = new SheduleService(_hubContext);

            // Send the notification to the provided route asynchronously
            await backgroundService.SendNotificationAsync(route, notification);
        }

        /// <summary>
        /// Method that is called when a client connects to the Hub.
        /// Validates the client before allowing the connection.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            try
            {
                // Retrieve clientId from claims
                var clientId = Context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                // Check if the clientId is valid by querying the database
                bool isValidClient = _dbContext.Users.Any(x => x.Id.ToString() == clientId);

                // If clientId is invalid, disconnect the client with an error message
                if (string.IsNullOrEmpty(clientId) || !isValidClient)
                {
                    var data = new Notification("Invalid ClientId", DateTime.Now);
                    await Clients.Caller.SendAsync("ConnectionError", data);
                    Context.Abort(); // Disconnect the client
                }
                else
                {
                    // Log successful connection to the console
                    Console.WriteLine($"Client connected: {Context.ConnectionId}");
                    await base.OnConnectedAsync();
                }
            }
            catch (Exception exp)
            {
                // Handle any errors during connection
                var data = new Notification(exp.Message, DateTime.Now);
                await Clients.Caller.SendAsync("ConnectionError", data);
                Context.Abort(); // Disconnect the client
            }
        }
    }
}
