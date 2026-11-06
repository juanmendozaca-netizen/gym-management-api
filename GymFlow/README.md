# GymFlow

Web API REST para la gestión de membresías y socios de un gimnasio. El proyecto utiliza ASP.NET Core .NET 10, Entity Framework Core y SQL Server.

## Modelo

- `Membresia`: plan contratado por el socio.
- `Socio`: persona registrada en el gimnasio.
- Relación 1:N: una membresía puede tener varios socios.

Reglas de negocio:

- No se permite repetir el nombre de una membresía.
- No se permite repetir el DNI o correo de un socio.
- No se puede eliminar una membresía que tenga socios asociados.

## Ejecución local

1. Tener disponible SQL Server LocalDB con la instancia `(localdb)\\MSSQLLocalDB`.
2. Ejecutar `Database/GymFlow.sql` en SQL Server Management Studio para crear `GymFlowDb`.
3. Configurar el secreto local:

```powershell
dotnet user-secrets set "ConnectionStrings:GymFlowDb" "Server=(localdb)\\MSSQLLocalDB;Database=GymFlowDb;Trusted_Connection=True;TrustServerCertificate=True;" --project GymFlow.csproj
```

4. Ejecutar la API:

```powershell
dotnet run --project GymFlow.csproj --urls http://localhost:5080
```

5. Abrir Swagger en `http://localhost:5080/swagger`.

## Endpoints principales

- `GET /api/Membresias`
- `GET /api/Membresias/{id}`
- `GET /api/Membresias/{id}/socios`
- `POST /api/Membresias`
- `PUT /api/Membresias/{id}`
- `DELETE /api/Membresias/{id}`
- `GET /api/Socios`
- `GET /api/Socios/{id}`
- `POST /api/Socios`
- `PUT /api/Socios/{id}`
- `DELETE /api/Socios/{id}`

Las pruebas reproducibles se encuentran en `GymFlow.http`.

## Configuración de producción

La base de datos de producción será Azure SQL Database y el backend se publicará en Render mediante el `Dockerfile` incluido.

Render debe recibir estas variables de entorno:

- `ConnectionStrings__GymFlowDb`: cadena de conexión de Azure SQL.
- `Cors__AllowedOrigin`: origen concreto autorizado por CORS.
- `ASPNETCORE_ENVIRONMENT`: `Production`.

No se deben subir contraseñas, cadenas de conexión ni tokens al repositorio.

## Despliegue

1. Crear la base de datos en Azure SQL Database.
2. Configurar la regla de firewall para permitir la conexión desde el servicio de hosting.
3. Ejecutar `Database/GymFlow.sql` en Azure SQL.
4. Subir el repositorio a GitHub.
5. Crear un Web Service en Render usando el `Dockerfile` ubicado en `GymFlow/Dockerfile`.
6. Configurar las variables de entorno indicadas.
7. Verificar Swagger en la URL pública `/swagger`.
8. Repetir las pruebas de `GymFlow.http` usando la URL pública.

## Respuestas HTTP demostradas

- `200 OK`: consultas exitosas.
- `201 Created`: creación de recursos.
- `204 NoContent`: actualización y eliminación correctas.
- `400 BadRequest`: datos inválidos, FK inexistente o ID inconsistente.
- `404 NotFound`: recurso inexistente.
- `409 Conflict`: duplicados o eliminación de una membresía con socios.
