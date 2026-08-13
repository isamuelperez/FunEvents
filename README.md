# FunEvents

Proyecto .NET 10 para gestionar eventos divertidos (FunEvents). Esta solución contiene una aplicación de consola y otros proyectos relacionados para ejecutar, desarrollar y probar funcionalidades del sistema.

## Características

- Aplicación de consola para interactuar con la lógica de FunEvents
- Código organizado en proyectos dentro de la solución FunEvents.slnx

## Requisitos

- .NET 10 SDK
- Visual Studio 2022/2026 o equivalente que soporte .NET 10

## Instalación

1. Clona el repositorio:

   git clone <URL-del-repositorio>

2. Entra en la carpeta del proyecto:

   cd FunEvents

3. Restaura paquetes y compila:

   dotnet restore
   dotnet build

## Ejecutar

Desde la carpeta de la solución o del proyecto de la aplicación de consola:

dotnet run --project FunEvents.Console

O abre la solución `FunEvents.slnx` en Visual Studio y ejecuta el proyecto de inicio.

## Estructura del repositorio

- FunEvents.slnx  - solución principal
- FunEvents.Console/ - proyecto de consola
- (otros proyectos) - librerías u otros módulos relacionados

## Arquitectura y patrones de diseño

El proyecto sigue una arquitectura limpia separando responsabilidades entre:

- Domain: entidades y tipos del dominio (FunEvents.Domain)
- Application: lógica de aplicación, casos de uso, comandos y consultas (CQRS) y abstractions (FunEvents.Application)
- Infrastructure: implementación de persistencia, EF Core DbContext, repositorios y migraciones (FunEvents.Infrasctructure)
- API / AppHost / Console: puntos de entrada y configuración de hosting (FunEvents.API, FunEvents.AppHost, FunEvents.Console)

Patrones y bibliotecas utilizadas:

- CQRS con MediatR: comandos y consultas manejados por handlers en la capa de Application
- Validación con FluentValidation
- Repositorio + Unit of Work: abstracciones en Application e implementaciones en Infrastructure
- Inyección de dependencias mediante Microsoft.Extensions.DependencyInjection
- Entity Framework Core para ORM y migraciones
- Patrones operativos: OpenTelemetry, health checks y descubrimiento de servicios a través de ServiceDefaults (extensiones en FunEvents.ServiceDefaults)

## Motor de base de datos

La solución está preparada para usar Microsoft SQL Server como proveedor de persistencia. Se usa Entity Framework Core con migrations incluidas (ver FunEvents.Infrasctructure/Data/Migrations) y el proyecto registra el DbContext para SQL Server en el arranque (AddSqlServerDbContext en el host/API).

Si se desea cambiar a otro proveedor (por ejemplo PostgreSQL o SQLite) hay que ajustar la configuración del DbContext y los paquetes de EF Core en el proyecto de Infrastructure.

## Contribuir

1. Crea una rama (branch):

   git checkout -b feature/mi-mejora

2. Realiza cambios, añade y comete:

   git add .
   git commit -m "Descripción de la mejora"

3. Abre un pull request hacia la rama principal del repositorio.

## Licencia

Indica aquí la licencia del proyecto (por ejemplo, MIT). Si no hay licencia, especifica que se contacte al autor para permisos.

## Contacto

Para dudas o incidencias abre un issue en el repositorio o contacta al responsable del proyecto.
