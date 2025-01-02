using Algorithms;
using IzukaBus;
using Microsoft.Extensions.Hosting;


var client = new HubClient("https://localhost:44316",
                            "e68cd019-32f7-493a-811a-6d929874878a",
                            "Principal160@");

await client.StartNotificationConnectionAsync();

 client.Subscribe("MyTopic",

	 async (model, ea) =>
	{
		// Simulating extracting and handling the message
		var message = model.Text;
		var timestamp = model.Date;

		// Handle the message
		Console.WriteLine(" [x] Received message: {0} at {1}", message, timestamp);
	});


 client.Subscribe("MyTopic2",

	 async (model, ea) =>
	 {
		 // Simulating extracting and handling the message
		 var message = model.Text;
		 var timestamp = model.Date;
		 // Handle the message
		 Console.WriteLine(" [x] Received message: {0} at {1}", message, timestamp);
	 });

while (true)
{
	await client.PublishAsync(  "MyTopic2", "Hello");

	// Wait for 10 seconds before repeating the task
	await Task.Delay(10000);  // 10 seconds delay
}



