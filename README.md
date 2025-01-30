# PriceStream Project

## Overview

**PriceStream** is a high-performance real-time price streaming service that provides both **REST API** and **WebSocket** endpoints for live financial instrument prices. The service sources data from a public market data provider, such as Binance, and efficiently manages over **1,000+ WebSocket subscribers**.

### Key Features:
1. **REST API**
   - Fetch a list of available financial instruments (e.g., EUR/USDT, USDT/JPY, BTC/USDT).
   - Retrieve the current price of a specific financial instrument.

2. **WebSocket Service**
   - Subscribe to live price updates for selected financial instruments.
   - Efficiently broadcast price updates to all subscribed clients.

3. **Data Source**
   - Prices are sourced from Binance WebSocket stream (`wss://stream.binance.com:443/ws/btcusdt`).

4. **Performance Optimization**
   - Efficiently handle **1,000+ WebSocket connections** using optimized broadcasting and concurrency management.

5. **Logging & Error Reporting**
   - Implements event and error logging for monitoring and debugging purposes.

---
## Features

- **Domain-Driven Design (DDD) Architecture**
- **CQRS with MediatR** for handling commands and queries
- **SignalR** for real-time price updates
- **Dependency Injection (DI)** for better maintainability
- **OpenAPI/Swagger** for API documentation
- **Health Checks** for monitoring

## Tech Stack

- **.NET 7+**
- **MediatR**
- **SignalR**
- **Swagger/OpenAPI**
- **Logging**

## Setup

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/PriceStream.git
   cd PriceStream
   ```
2. Install dependencies:
   ```sh
   dotnet restore
   ```
3. Build and run:
   ```sh
   dotnet run
   ```
---

## 1. Software Architecture

The **PriceStream** project follows a modular and scalable software architecture. Given the presence of Domain-Driven Design (DDD) patterns, the architecture is likely structured in layers, such as:

- **Presentation Layer**: Handles WebSocket interactions.
- **Application Layer**: Coordinates application logic and use cases.
- **Domain Layer**: Contains business logic, entities, and domain services.
- **Infrastructure Layer**: Manages WebSocket communication and in-memory caching.

This architecture promotes **separation of concerns** and ensures scalability, maintainability, and testability.

## 2. DDD Keeping SOLID Principles

By following DDD and SOLID principles, the project remains **scalable, flexible, and maintainable**.

## 3. WebSocket Services & Price Caching

### ExchangeWebSocketBackgroundService

This background service is responsible for maintaining WebSocket connections to various exchanges. It continuously listens for price updates and forwards them to the appropriate handlers. Key features include:

- Establishing and maintaining WebSocket connections.
- Handling reconnections in case of failures.
- Dispatching incoming data to appropriate services.

### BinanceWebSocketService

A specialized WebSocket service that connects directly to Binance exchange streams. It provides:

- Real-time market data streaming from Binance.
- Efficient parsing and normalization of price updates.
- Forwarding price updates to the in-memory cache.

### WebSocketHandler

This component acts as a mediator, routing WebSocket messages to the correct services. Its primary roles include:

- Managing active WebSocket clients.
- Broadcasting price updates to connected clients concurrently to improve performance.
- Handling subscription and unsubscription requests from clients.

### PriceCache

Since the project does not use a traditional database, **PriceCache** is the central component for storing and retrieving real-time price data in-memory. Features:

- High-speed lookups for latest price updates.
- Lightweight and optimized for real-time performance.
- Data expiration strategies to prevent memory overflow.

### Concurrently Broadcasting for Performance

To maximize performance, the **WebSocketHandler** uses **concurrent broadcasting** to send updates to multiple clients simultaneously. This ensures:

- **Low Latency**: Clients receive real-time updates without delay.
- **Efficient Resource Utilization**: Reduces the time spent iterating over client connections.
- **Scalability**: Supports a large number of WebSocket clients without performance degradation.

Using asynchronous operations and parallel processing techniques, the system can handle high-throughput market data efficiently.

## 4. ExchangeWebSocketClient

The **ExchangeWebSocketClient** is a separate project within the solution that acts as a client, listening to the **PriceStream** server. Its main responsibilities include:

- Establishing a WebSocket connection with the **PriceStream** server.
- Subscribing to price updates for various trading pairs.
- Handling real-time price messages.

The **ExchangeWebSocketClient** ensures efficient consumption of market data while keeping network resource usage optimized.

## 5. Docker & Containerization

### Dockerfile

The **Dockerfile** in this project defines the containerized environment for running the application. Typically, it includes:

- **Base Image**: Specifies the .NET runtime or SDK.
- **Copying Source Code**: Transfers application files into the container.
- **Building the Application**: Restores dependencies and compiles the project.
- **Setting Entry Point**: Defines the command that runs the application inside the container.

### Docker Compose

The **docker-compose.yml** file defines multi-container orchestration. It typically:

- Configures application services (e.g., WebSocket service, caching layer).
- Sets up environment variables and network configurations.
- Automates the deployment of multiple services using a single command.

### Running the Project with Docker

To build and run the project in a Docker container, use the following commands:

```sh
# Build the Docker image
docker build -t pricestream .

# Run the container
docker run -p 5000:5000 pricestream

# Using Docker Compose
docker-compose up -d
```

### Scaling with Docker Compose Replicas

To enhance scalability and handle a high number of WebSocket clients, **Docker Compose** can be used to run multiple replicas of the service. This is done using the `deploy` section in `docker-compose.yml`:

```yaml
services:
  websocketclient:
    deploy:
      replicas: 3  # Number of instances to run
```

### Benefits of Using Replicas

- **Load Distribution**: Requests are handled across multiple instances, reducing the load on a single service.
- **High Availability**: If one instance fails, others continue serving clients.
- **Better Performance**: Supports a larger number of WebSocket connections without degradation.

## Conclusion

The **PriceStream** project follows a well-structured **DDD-based architecture**, adhering to **SOLID principles** while leveraging **WebSocket services** and **in-memory caching** for real-time data processing. Additionally, **Docker** ensures smooth deployment and scalability, making the system highly efficient and reliable.

