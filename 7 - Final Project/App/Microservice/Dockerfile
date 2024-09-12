FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env

WORKDIR /base

COPY *.csproj .

RUN dotnet restore -r linux-musl-x64

COPY . .

RUN dotnet publish -c Release -o /publish -r linux-musl-x64 --no-restore

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64

WORKDIR /app

COPY --from=build-env publish .

EXPOSE 8080

ENTRYPOINT ["./Microservice"]
