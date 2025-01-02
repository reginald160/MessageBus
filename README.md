
# Message Broker with SignalR, Hangfire, and JWT Security

This project implements a real-time message broker system built using SignalR for communication, Hangfire for background job processing, and JWT for secure authentication. It includes both a server-side implementation and a client library to consume the messages.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Setup and Installation](#setup-and-installation)
- [Usage](#usage)
  - [Server](#server)
  - [Client](#client)
- [API Endpoints](#api-endpoints)
- [License](#license)

---

## Overview

This project is a SignalR-based message broker that facilitates real-time communication between clients and the server. It leverages:

- **SignalR** for real-time communication.
- **Hangfire** to schedule and execute background jobs.
- **JWT** (JSON Web Tokens) for secure authentication and authorization.

The solution is designed to be highly scalable and secure, making it suitable for scenarios requiring real-time notifications and background processing.

## Features

- **Real-time Communication**: Clients can send and receive messages or notifications instantly via SignalR.
- **Background Job Management**: Use Hangfire to process tasks asynchronously.
- **Secure Authentication**: JWT is used for client authentication, ensuring secure access to the broker.
- **Topic Subscription**: Clients can subscribe to topics and receive targeted notifications.

## Technologies Used

- ASP.NET Core SignalR
- Hangfire
- JSON Web Tokens (JWT)
- Entity Framework Core (EF Core) for database access
- .NET Core DI and logging frameworks

## Project Structure

### Server-Side
- **MessageHub**: Implements SignalR functionality for client-server communication.
- **AuthController**: Handles user authentication and client creation.
- **SheduleService**: Manages notifications and interactions with SignalR.

### Client-Side
- **HubClient**: A wrapper for connecting to the SignalR hub and handling notifications.
- **JWTHelper**: Utility for generating and validating JWT tokens.

## Setup and Installation

### Prerequisites

- .NET 5.0 SDK and .NET 8.0 SDK
- SQL Server (or any other database supported by EF Core)
- Visual Studio or VS Code

### Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/reginald160/MessageBus.git
   ```

2. **Configure the Database**
   Update the connection string in `appsettings.json` to point to your database.

3. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Start the Server**
   ```bash
   dotnet run --project IzukaBus
   ```

5. **Build and Run the Client**
   Configure your client project with the appropriate `hostname`, `clientId`, and `clientSecretKey`. Then run it.

## Usage

### Client

#### Initializing the Client
```csharp
var hubClient = new HubClient("https://localhost:44316", "your-client-id", "your-secret-key");
hubClient.Subscribe("topic-name", async (notification, args) => {
    Console.WriteLine($"Received notification: {notification.Text}");
});
await hubClient.StartNotificationConnectionAsync();
```

#### Publishing Messages
```csharp
await hubClient.PublishAsync("topic-name", "Your message here");
```

## API Endpoints

### Authentication
#### POST `api/Auth/CreateClient`
- **Description**: Registers a new client.
- **Payload**:
  ```json
  {
    "Email": "client@example.com",
    "Password": "securepassword"
  }
  ```

#### POST `api/Auth/Authenticate`
- **Description**: Authenticates a client and returns a JWT token.
- **Payload**:
  ```json
  {
    "ClientId": "client-id",
    "SecretKey": "securepassword"
  }
  ```
