# SlideCloud
(SlideCloud.ir)
SlideCloud is a modern web application built with .NET 8.0, following clean architecture principles and containerized with Docker.

## 🚀 Features

- Built with .NET 8.0
- Clean Architecture implementation
- Docker containerization
- HTTPS support
- Modern web application structure

## 🏗️ Project Structure

The solution follows a clean architecture pattern with the following layers:

```
Src/
├── Core/
│   ├── SlideCloud.Domain/        # Domain entities and business rules
│   └── SlideCloud.Application/   # Application business rules and use cases
├── Infrastructure/
│   └── SlideCloud.Infrastructure/# External concerns implementation
└── Presentation/
    └── SlideCloud.Web/          # Web API and UI layer
```

## 🛠️ Prerequisites

- .NET 8.0 SDK
- Docker (for containerized deployment)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### Local Development

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd SlideCloud
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project Src/Presentation/SlideCloud.Web
   ```

### Docker Deployment

1. Build the Docker image:
   ```bash
   docker build -t slidecloud .
   ```

2. Run the container:
   ```bash
   docker run -p 8080:8080 -p 8081:8081 slidecloud
   ```

## 🔧 Configuration

The application exposes two ports:
- 8080: Main application port
- 8081: HTTPS port

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Authors

- Your Name - Initial work

## 🙏 Acknowledgments

- .NET Team for the amazing framework
- Docker team for containerization support 