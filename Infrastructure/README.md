# migrations

To create new migration:
```bash
dotnet ef migrations add <migration-name> --project Infrastructure --startup-project Api
```

To update database
```bash
dotnet ef database update --project Infrastructure --startup-project API
```