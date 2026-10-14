# Hibana

## Tech stack
- .NET 10 [ASP.NET Core SDK](https://dotnet.microsoft.com/en-us/)
- Postgre SQL [Npgsql connector](http://www.npgsql.org/)
- Redis [Stack exchange connector](https://stackexchange.github.io/StackExchange.Redis/)
- RabbitMQ [AMQP client](https://www.rabbitmq.com/client-libraries/amqp-client-libraries)
- Auth0 [SaaS Middleware](https://auth0.com/)

## Deployment
- local docker compose containers [docker engine (desktop on windows)](https://www.docker.com)
- cloud kubernetes cluster using [k3s distribution](https://k3s.io)

## Instructions
- compose.yaml contains docker services layout
- .env contains the parameters of said services
- run with 'docker compose -f compose.yaml up -d' to launch docker services(database, cache, messaging)

## Hibana.sln contains
- AuthService running on port 4000
- UserService running on port 5000
- IdentityService running on port 5001