# Online Bidding System - Modular Monolith

A comprehensive online bidding system built as a modular monolith using .NET 8, following clean architecture principles and domain-driven design.

## System Overview

This application implements an online bidding system with the following key features:
- User authentication and authorization
- Auction creation and management
- Real-time bidding with WebSocket support
- Payment processing
- Notification system
- Search and filtering capabilities

## Architecture

The system is built as a modular monolith with the following modules:

### Core Modules
1. **User Management** - User registration, authentication, profiles
2. **Auction Management** - Auction CRUD, status management
3. **Bidding** - Bid placement, validation, real-time updates
4. **Payment** - Payment processing, transaction management
5. **Notification** - Real-time notifications, email/SMS
6. **Search** - Auction search, filtering, recommendations

### Technical Stack
- **.NET 8** - Backend framework
- **Entity Framework Core** - ORM
- **SQL Server** - Primary database
- **Redis** - Caching and session storage
- **SignalR** - Real-time communication
- **FluentValidation** - Input validation
- **Serilog** - Logging
- **Swagger** - API documentation

## Project Structure

```
BiddingSystem/
├── src/
│   ├── BiddingSystem.API/              # Web API project
│   ├── BiddingSystem.Core/             # Domain entities and interfaces
│   ├── BiddingSystem.Infrastructure/   # Data access and external services
│   ├── BiddingSystem.Application/      # Business logic and services
│   └── BiddingSystem.Shared/           # Shared DTOs and utilities
├── tests/
│   ├── BiddingSystem.UnitTests/
│   └── BiddingSystem.IntegrationTests/
├── docs/                               # Documentation
└── docker/                             # Docker configuration
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Redis
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Update connection strings in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Start the application: `dotnet run`

### API Documentation
Once the application is running, visit `/swagger` for interactive API documentation.

## Key Features

### Real-time Bidding
- WebSocket-based real-time bid updates
- Automatic bid validation and conflict resolution
- Live auction status updates

### Payment Integration
- Secure payment processing
- Multiple payment method support
- Transaction history and reconciliation

### Search and Discovery
- Advanced auction search with filters
- Recommendation engine
- Category-based browsing

### Notification System
- Real-time notifications via WebSocket
- Email notifications for important events
- SMS notifications for critical updates

## Development Guidelines

### Code Organization
- Follow Clean Architecture principles
- Use Domain-Driven Design patterns
- Implement CQRS pattern for complex operations
- Use dependency injection throughout

### Testing
- Unit tests for business logic
- Integration tests for API endpoints
- End-to-end tests for critical user flows

### Security
- JWT-based authentication
- Role-based authorization
- Input validation and sanitization
- SQL injection prevention

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License. 