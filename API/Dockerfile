FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

COPY API/API.csproj ./API/
COPY Core/Core.csproj ./Core/

RUN dotnet restore ./API/API.csproj

COPY API/ ./API/
COPY Core/ ./Core/

WORKDIR /src/API

ENTRYPOINT ["dotnet", "run", "--no-launch-profile", "--non-interactive", "--urls", "http://0.0.0.0:3000"]

# można dodać "watch" jeżeli ktoś chce używać dotnet watcha na wzór node'a