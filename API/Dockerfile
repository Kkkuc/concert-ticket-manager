FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore ./API/API.csproj

COPY . .

WORKDIR /src/API

ENTRYPOINT ["dotnet", "run", "--no-launch-profile", "--non-interactive", "--urls", "http://0.0.0.0:3000"]

# można dodać "watch" jeżeli ktoś chce używać dotnet watcha na wzór node'a