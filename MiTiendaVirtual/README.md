# Tienda (ASP.NET Core MVC, .NET 10, EF Core)

1. Ajusta `ConnectionStrings:DefaultConnection` en `appsettings.json`.
2. `dotnet run`
3. Al iniciar se crea la base de datos (si no existe) y se cargan datos de ejemplo.

Todo el sitio exige iniciar sesión (tabla Usuario, contraseñas con hash PBKDF2).
Si la tabla Usuario está vacía se crea: admin@example.com / Admin123*
