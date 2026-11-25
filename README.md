# concert-ticket-manager
App for team project

remember to use
```bash
dotnet restore
```
(rider does it automaticly)
Migration how to is in `Infrastructure/README.md`

# Connection to database (Polish version below)

### Requirements
- Docker and Docker Compose (e.g. Docker Desktop)
- `.env` file with database settings and a JWT signing key
- .NET 10 SDK when running the backend directly through an IDE

### 1. Environment variables (Docker)
In the root directory of the repository, create a `.env` file:
```env
POSTGRES_DB=concert_ticket_manager
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_password
Jwt__Key=your_generated_base64_key
```

The JWT key is required to start the backend. It must be a randomly generated key (at least 32 bytes, encoded in Base64).

For example, generate one using PowerShell:
```powershell
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

Copy the generated value into `Jwt__Key`.

### 2. Build

```bash
docker compose build
docker compose up
```

Alternatively, build and start the containers in the background:
```bash
docker compose up -d --build
```


### 3. Workflow
You can work entirely using Docker, though it is not recommended because after every code change you have to run:

```bash
docker compose restart backend
```

Alternatively, if you prefer that workflow, you can add the "watch" flag to the ENTRYPOINT in `API/Dockerfile`:
```API/Dockerfile
ENTRYPOINT ["dotnet", "watch", "run", "etc"]
```

There is a 3rd option, which is running the backend directly via your IDE — for this, you need to configure the connection string.

### 4. Working via IDE appsettings.Development.json
You can edit the file mentioned above, just make sure not to commit it to Git!

```appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=your_user;Password=your_password"
  }
}
```

### 5. Working via IDE user-secrets

In this case, leave `appsettings.Development.json` untouched.

wpisujemy
```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=your_user;Password=your_password"
dotnet user-secrets set "Jwt:Key" "your_generated_base64_key"
```

Alternatively, you can set them through your IDE, e.g., in Rider:API -> Tools -> .Net User Secrets
and paste the following:

```secrets.json
{
  "Jwt": {
    "Key": "your_generated_base64_key"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=your_user;Password=your_password"
  }
}
```

Due to Windows enforcing localhost, you can access our services here:


  - Frontend: `http://192.168.25.50:5000`
  - Backend: `http://192.168.25.100:3000`
  - Database `postgres://192.168.25.150:5432`

if not, they will be here

  - Frontend: `http://localhost:5000`
  - Backend: `http://localhost:3000`
  - Database `postgres://localhost:5432`



# Łączenie bazy danych

### 1. Zmienne środowiskowe (Docker)
W głównym katalogu repozytorium utwórz plik `.env`:
```env
POSTGRES_DB=concert_ticket_manager
POSTGRES_USER=twoj_user
POSTGRES_PASSWORD=twoje_haslo
Jwt__Key=twoj_wygenerowany_klucz_base64
```

Klucz JWT jest wymagany do uruchomienia backendu. Powinien być wygenerowany losowo, mieć co najmniej 32 bajty i być zapisany w formacie Base64.

Można go wygenerować w PowerShell:
```powershell
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

Wygenerowaną wartość wpisujemy do `Jwt__Key`.

### 2. Build

```bash
docker compose build
docker compose up
```

Można też zbudować i uruchomić kontenery w tle:
```bash
docker compose up -d --build
```

### 3. Praca
Można albo pracować full na dockerze nie polecam bo trzeba po każdej zmianie wpisać

```bash
docker compose restart backend
```

ewentualnie jak tak chcecie to można dopisać flagę "watch" w ENTRYPOINT w `API/DockerFile`
```API/Dockerfile
ENTRYPOINT ["dotnet", "watch", "run", "itp"]
```

Jest 3cia opcja czyli praca z backendem przez środowisko i do niej trzeba skonfigurować connectionstringa

### 4. Praca przez środowisko `appsettings.Development.json`

Można zedytować plik powyżej wymieniony tylko nie dodawać go na gita!

```appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=twoj_user;Password=twoje_haslo"
  }
}
```

### 5. Praca przez środowisko user-secrets

W tym przypadku nic nie zmieniamy w appsettings.Development.json.

wpisujemy
```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=twoj_user;Password=twoje_haslo"
dotnet user-secrets set "Jwt:Key" "twoj_wygenerowany_klucz_base64"
```

ewentualnie można je setować przez środowisko,
Np. w Riderze
API -> Tools -> .Net User Secrets
i tam wklejamy to

```secrets.json
{
  "Jwt": {
    "Key": "twoj_wygenerowany_klucz_base64"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=concert_ticket_manager;Username=twoj_user;Password=twoje_haslo"
  }
}
```

Przez windowsa i jego wymuszanie localhosta możemy znaleźć nasze serwisy tutaj


  - Frontend: `http://192.168.25.50:5000`
  - Backend: `http://192.168.25.100:3000`
  - Database `postgres://192.168.25.150:5432`

 a jeżeli się nie da to tutaj

  - Frontend: `http://localhost:5000`
  - Backend: `http://localhost:3000`
  - Database `postgres://localhost:5432`
