<!-- ABOUT THE PROJECT -->
# JWT Authentication for HTTP APIs

JSON Web Token (JWT) is an open standard (RFC 7519) that defines a compact and self-contained way for securely transmitting information between parties as a JSON object.
This information can be verified and trusted because it is digitally signed.

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

Things you need to use the software and how to install them.
* [Visual Studio / Visual Studio Code](https://visualstudio.microsoft.com/)
* [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet)
* [Docker](https://www.docker.com/)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/gitViwe/JWTAuthentication.git
   ```
2. Generate certificate. [Starting a container with https support using docker compose](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-5.0#starting-a-container-with-https-support-using-docker-compose)
   ```
   dotnet dev-certs https -ep .aspnet\https\jwt-authentication.pfx -p password!
   dotnet dev-certs https --trust
   ```
3. Run via Docker
   ```
   docker compose up --build -d
   ```
   
Explore the Swagger API documentation: [HTTP endpoint](http://localhost:5161/swagger) or [HTTPS endpoint](https://localhost:7161/swagger)

OpenTelemetry integration with Jeager UI: [localhost:16686](http://localhost:16686/)
