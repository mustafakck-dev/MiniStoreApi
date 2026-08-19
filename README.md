# MiniStoreApi

MiniStoreApi is a backend-focused e-commerce Web API developed with ASP.NET Core as a learning project for applying common backend development concepts and infrastructure tools in a single application.

The project follows a layered architecture and includes authentication and authorization, product and category management, order processing, searching, filtering, pagination, distributed caching with Redis, asynchronous messaging with RabbitMQ, automated tests, and Docker-based containerization.

## Tech Stack

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- AutoMapper
- Redis
- RabbitMQ
- Docker
- Docker Compose
- xUnit
- Moq

## Architecture

The application is organized using a layered architecture.

```text
Client / Swagger
        |
        v
Presentation
Controllers
        |
        v
Services
Business Logic
        |
        v
Repositories
Data Access
        |
        v
Entity Framework Core
        |
        v
SQL Server
```

Main projects:

```text
MiniStoreApi
|
|-- Entities
|   |-- Models
|   |-- DTOs
|   |-- Exceptions
|   |-- RequestFeatures
|   |-- MessageModels
|
|-- Repositories
|   |-- Contracts
|   |-- EFCore
|   |-- Extensions
|   |-- Migrations
|
|-- Services
|   |-- Contracts
|   |-- Mapping
|   |-- Messaging
|
|-- Presentation
|   |-- Controllers
|
|-- WebApi
|   |-- Program.cs
|   |-- Extensions
|   |-- Data
|
|-- MiniStore.Worker
|
|-- MiniStoreApi.Tests
|
|-- MiniStoreApi.IntegrationTests
```

## Features

### Product Management

The API provides product operations including:

- Product creation
- Product retrieval
- Product updates
- Product deletion
- Category-based filtering
- Price filtering
- Searching
- Sorting
- Pagination

Pagination metadata is returned through the `X-Pagination` response header.

Example:

```http
GET /api/products?pageNumber=1&pageSize=10
```

Additional query parameters can be used for searching, filtering, and sorting products.

### Category Management

Categories are managed separately and are associated with products through Entity Framework Core relationships.

### Authentication

Authentication is implemented using ASP.NET Core Identity and JWT.

The authentication flow is:

```text
User Login
    |
    v
ASP.NET Core Identity
    |
    v
Credentials Validated
    |
    v
JWT Generated
    |
    v
Client sends JWT with requests
```

Authenticated requests use:

```http
Authorization: Bearer <token>
```

### Role-Based Authorization

The application supports role-based authorization.

Current roles include:

```text
User
Admin
```

Product modification operations are restricted to administrators.

```text
GET     -> Public
POST    -> Admin
PUT     -> Admin
DELETE  -> Admin
```

### Order Processing

Authenticated users can create orders containing multiple items.

The order workflow includes:

```text
Create Order Request
        |
        v
Validate Products
        |
        v
Check Stock
        |
        v
Decrease Stock
        |
        v
Create Order Items
        |
        v
Calculate Total Price
        |
        v
Save Order
```

Custom exceptions are used for scenarios such as:

- Product not found
- Category not found
- Order not found
- Invalid price range
- Insufficient stock

A centralized exception handler converts application exceptions into appropriate HTTP responses.

## Redis Caching

Redis is used as a distributed cache.

The project applies the cache-aside pattern for frequently requested product data.

```text
GET Product
     |
     v
Check Redis
  /       \
Hit       Miss
 |          |
Return    SQL Server
            |
            v
         Cache Data
            |
            v
          Return
```

Cache invalidation is performed when product data changes to prevent stale data from being returned.

## RabbitMQ Messaging

RabbitMQ is used for asynchronous order-created messages.

Producer flow:

```text
OrderService
     |
     v
OrderCreatedMessage
     |
     v
RabbitMqPublisher
     |
     v
ministore.orders
Exchange
     |
     | order.created
     v
order-created
Queue
```

Consumer flow:

```text
order-created Queue
        |
        v
MiniStore.Worker
        |
        v
Deserialize Message
        |
        v
Process Message
        |
        v
ACK
```

The Worker uses manual acknowledgements so that messages are acknowledged only after successful processing.

Basic NACK and requeue behavior is also implemented for failed message processing.

## Docker

The main infrastructure can be started using Docker Compose.

The Docker environment contains:

```text
ASP.NET Core API
SQL Server
Redis
RabbitMQ
```

### Environment Variables

Copy `.env.example`:

```bash
cp .env.example .env
```

On Windows, you can also create a `.env` file manually from `.env.example`.

Example configuration:

```env
MSSQL_SA_PASSWORD=YourStrongPasswordHere
DATABASE_NAME=MiniStoreDb
DATABASE_USER=sa
API_PORT=8081
DATABASE_PORT=1433

RABBITMQ_USER=ministore
RABBITMQ_PASSWORD=YourRabbitMqPasswordHere
```

The real `.env` file is excluded from Git.

### Start Containers

```bash
docker compose up -d --build
```

Check the running containers:

```bash
docker compose ps
```

The API is available by default at:

```text
http://localhost:8081
```

RabbitMQ Management UI is available at:

```text
http://localhost:15672
```

## RabbitMQ Initial Setup

The current RabbitMQ implementation expects the following topology:

### Exchange

```text
ministore.orders
```

### Queue

```text
order-created
```

### Routing Key

```text
order.created
```

Binding:

```text
ministore.orders
        |
        | order.created
        v
order-created
```

These can be configured through the RabbitMQ Management UI.

## Running the Worker

The background consumer is located in:

```text
MiniStore.Worker
```

Before running the Worker locally, configure the RabbitMQ credentials using environment variables.

PowerShell example:

```powershell
$env:RabbitMq__Host="localhost"
$env:RabbitMq__UserName="ministore"
$env:RabbitMq__Password="YourRabbitMqPasswordHere"

dotnet run --project MiniStore.Worker
```

When running correctly:

```text
Worker order-created queue'sunu dinliyor.
```

After an order is created, the Worker consumes the `OrderCreatedMessage` and acknowledges it.

## Database

SQL Server is used as the relational database.

Entity Framework Core is used for:

- Entity mapping
- Relationships
- LINQ queries
- Migrations
- Database access
- Change tracking

Database migrations are stored under:

```text
Repositories/Migrations
```

## Testing

The solution contains both unit tests and integration tests.

### Unit Tests

Unit tests focus mainly on service-layer business logic using:

- xUnit
- Moq

Dependencies such as repositories, AutoMapper, logging, and cache services are mocked.

### Integration Tests

Integration tests validate multiple application components working together, including:

```text
HTTP Request
    |
    v
Middleware
    |
    v
Authentication / Authorization
    |
    v
Controller
    |
    v
Service
    |
    v
Repository
    |
    v
Database
```

Run all tests:

```bash
dotnet test MiniStoreApi.slnx
```

Current result:

```text
Total tests: 16
Passed: 16
Failed: 0
```

Build the solution:

```bash
dotnet build MiniStoreApi.slnx
```

## Security

Sensitive values such as database passwords, RabbitMQ credentials, and JWT secrets should not be committed to source control.

The repository excludes local secrets through `.gitignore`.

Files such as the following should remain local:

```text
.env
appsettings.Development.json
```

Example configuration files contain placeholder values only.

## Project Purpose

MiniStoreApi was developed to practice and demonstrate common backend engineering concepts beyond basic CRUD operations.

The project focuses on:

- Layered application design
- RESTful API development
- Dependency Injection
- Entity Framework Core
- Authentication and authorization
- Business rules
- Error handling
- Searching and filtering
- Pagination
- Distributed caching
- Asynchronous messaging
- Background processing
- Containerization
- Unit testing
- Integration testing