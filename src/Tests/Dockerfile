# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS test

COPY . /source
WORKDIR /source

RUN dotnet restore src/Tests/RegistroContatoTestes.csproj

RUN dotnet build src/Tests/RegistroContatoTestes.csproj -c Release

RUN dotnet publish src/Tests/RegistroContatoTestes.csproj -c Release -o /app/tests

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS final

WORKDIR /app

COPY --from=test /app/tests .

# Executa os testes automaticamente ao subir o container
ENTRYPOINT ["dotnet", "test", "RegistroContatoTestes.dll", "--logger", "trx"]
