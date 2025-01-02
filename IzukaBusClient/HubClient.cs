using Algorithms;
using IzukaBusClient;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace IzukaBus
{
    public class HubClient : IAsyncDisposable
    {
        // Record for Notification model, with default values for topic
        public record Notification(string Text, DateTime Date, string Topic = "");

        // Private fields for HubClient class
        private readonly string _hostname; // The hostname of the server
        private readonly string _clientId; // Client identifier
        private readonly string _clientSecretKey; // Key used to generate JWT
        private HubConnection _hubConnection; // Connection to the SignalR hub
        public Consumer ConsumerContext { get; set; } // Optional consumer context

        // Event triggered when a notification is received
        public event Func<Notification, EventArgs, Task> NotificationReceived;

        // The access token generated from client credentials
        public string AccessToken { get; private set; }

        private readonly ILogger<HubClient> _logger; // Logger for debugging and tracking errors

        // Constructor for initializing the HubClient
        public HubClient(string hostname, string clientId, string clientSecretKey)
        {
            _hostname = hostname ?? throw new ArgumentNullException(nameof(hostname)); // Ensure hostname is not null
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId)); // Ensure clientId is not null
            _clientSecretKey = clientSecretKey ?? throw new ArgumentNullException(nameof(clientSecretKey)); // Ensure secret key is not null


            if (string.IsNullOrEmpty(_clientId))
            {
                // Ensure clientId is not empty
                throw new ArgumentException("ClientId cannot be empty.", nameof(clientId)); 
            }

            // Generate the JWT access token using client credentials
            AccessToken = JWTHelper.GetAccessToken(_clientId, hostname, _clientSecretKey).Result;

            // Initialize the SignalR Hub connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(new Uri($"{_hostname}/hub/event"), options =>
                {
                    // Set the AccessTokenProvider for token-based authentication
                    options.AccessTokenProvider = () => Task.FromResult(AccessToken);
                })
                .WithAutomaticReconnect() // Enable automatic reconnection
                .Build();

            // Register an event handler for the "ConnectionError" method on the Hub
            _hubConnection.On<Notification>("ConnectionError", HandleConnectionError);

            //_logger.LogInformation("HubClient initialized for clientId: {ClientId}", _clientId); // Log initialization
        }

        // Starts the SignalR connection asynchronously
        public Task StartNotificationConnectionAsync() =>
            _hubConnection.StartAsync();

        // Subscribe to a specific topic for receiving notifications
        public void Subscribe(string topic, Func<Notification, EventArgs, Task> eventHandler)
        {
            // Create a unique method name per client/topic
            string methodName = $"{topic}-{_clientId}"; 

            // Register a callback for the specific topic using the method name
            _hubConnection.On<Notification>(methodName, async (Notification notification) =>
            {
                if (eventHandler != null)
                {
                    var eventArgs = new EventArgs(); // Optionally, create custom event args here
                    await eventHandler(notification, eventArgs); // Invoke the event handler
                }
            });

            //_logger.LogInformation("Subscribed to topic: {Topic}", topic); // Log subscription
        }

        // Publishes a notification to a specific topic
        public async Task PublishAsync(string topic, string messageBody)
        {
            try
            {
                // Publish a notification to the specified topic
                await _hubConnection.InvokeAsync("PublishNotification", new Notification(messageBody, DateTime.UtcNow, topic));
               // _logger.LogInformation("Notification published to topic: {Topic}", topic); // Log successful publish
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error while publishing notification to topic: {Topic}", topic); // Log error during publishing
            }
        }

        // Handles connection errors and throws an UnauthorizedAccessException
        private void HandleConnectionError(Notification notification)
        {
            //_logger.LogError("Connection error: {ErrorMessage}", notification.Text); // Log the error
            throw new UnauthorizedAccessException(notification.Text); // Throw exception with error message
        }

        // Clean up resources when disposing of the HubClient instance
        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync(); 
                _hubConnection = null; 
               // _logger.LogInformation("HubClient connection disposed."); // Log disposal
            }
        }
    }
}
