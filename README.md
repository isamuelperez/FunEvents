# FunEvents

## Requerimientos Funcionales

- 1er Punto.
Describir la arquitectura general de un sistema de venta de entradas para una empresa ficticia FunEvents que
organiza eventos de entretenimiento (conciertos, teatro, etc.)El sistema debe gestionar la venta de entradas
de distintos canales. El canal principal es la venta online a través del portal de la empresa, pero también se
pueden vender entradas desde las oficinas de atención al cliente existentes por todo el mundo y desde
portales u oficinas de colaboradores. Habitualmente estos colaboradores quieren personalizar la experiencia
de los compradores integrando el sistema de FunEvents con su propio portal o aplicación de punto de venta.

- 2do Punto.
Implementar un prototipo mínimo de un cliente de consola para pruebas que realiza la reserva de entradas
para un evento a partir de un código de evento y de usuario ya conocidos. Esta reserva debe realizarse a través
de un api web que también debe implementar el candidato.

- Tecnologías a usar : Para el prototipo se valorará usar una versión moderna de .NET (netcore 8 o superior) y
ASP.NET Core Minimal APIs. Si se considera necesario, usar Postgres como base de datos y .NET Aspire como
orquestador.

Prototipo de un sistema de gestión y reserva de entradas para **FunEvents**, empresa ficticia dedicada a organizar eventos de entretenimiento como conciertos, teatro y otros espectáculos.

## 1. Descripción

FunEvents debe permitir gestionar la venta/reserva de entradas desde diferentes canales:

- Portal web de FunEvents.
- Oficinas de atención al cliente.
- Portales de colaboradores.
- Aplicaciones/Puntos de colaboradores.

La arquitectura centraliza las reglas de negocio en una API de FunEvents para que los distintos canales consuman los mismos servicios sin acceder directamente a la base de datos.

```text
Portal Web ───────────────┐
Oficinas FunEvents ───────┤
Portal colaborador ───────┤──> FunEvents API ──> Application ──> Domain
POS colaborador ──────────┘                              │
                                                        ▼
                                                  Infrastructure
                                                        │
                                                        ▼
                                                    SQL Server
```

## 2. Arquitectura

La solución utiliza una arquitectura basada en **Clean Architecture**, separando responsabilidades en:

```text
FunEvents
├── FunEvents.API
├── FunEvents.Application
├── FunEvents.Domain
├── FunEvents.Infrastructure
├── FunEvents.Console
├── FunEvents.AppHost
└── FunEvents.ServiceDefaults
```

### Domain

Contiene el núcleo del negocio y no depende de infraestructura ni de ASP.NET Core.

Entidades principales:

- `Event`
- `User`
- `Reservation`

### Application

Contiene los casos de uso de la aplicación:

- Commands.
- Queries.
- Handlers.
- Validators.
- DTOs/Results.
- Abstracciones de repositorios.

Se utiliza **CQRS** y **MediatR**.

**CQRS** organiza los casos de uso como Commands y Queries, mientras que **MediatR** se encarga de enviar esos Commands/Queries hacia sus respectivos Handlers.

Ejemplo:

```text
CreateReservationCommand
        │
        ▼
CreateReservationHandler
```

### Infrastructure

Contiene las implementaciones relacionadas con persistencia:

- Entity Framework Core.
- `FunEventsDbContext`.
- Configuraciones de entidades (**Fluent API**).
- Repositories.
- Unit of Work.
- Transacciones.
- Migraciones.

### API

La presentación está implementada con **ASP.NET Core Minimal APIs**.

Responsabilidades:

- Exponer endpoints HTTP.
- Recibir requests.
- Enviar Commands/Queries a Application.
- Convertir resultados en respuestas HTTP.

### Console

Cliente de consola utilizado para probar la reserva mediante HTTP.

```text
FunEvents.Console
       │
       │ HTTP
       ▼
FunEvents.API
```

### AppHost

Utiliza **.NET Aspire** para orquestar la API y SQL Server.

```text
AppHost
 ├── FunEvents API
 └── SQL Server
       └── funevents
```

## 3. Flujo de una reserva

El caso de uso principal es crear una reserva para un usuario y un evento conocidos.

```text
Cliente Console / Portal / POS
              │
              │ POST /api/reservations
              ▼
       Minimal API Endpoint
              │
              ▼
       FluentValidation
              │
              ▼
           MediatR
              │
              ▼
 CreateReservationHandler
              │
       ┌──────┴──────┐
       ▼             ▼
 UserRepository   EventRepository
       │             │
       └──────┬──────┘
              ▼
     Verificar disponibilidad
              │
              ▼
        Iniciar transacción
              │
       ┌──────┴──────┐
       ▼             ▼
Actualizar evento  Crear reserva
       │             │
       └──────┬──────┘
              ▼
        SaveChanges
              │
              ▼
           Commit
              │
              ▼
       HTTP 201 Created
```

### Paso a paso

1. El cliente envía código de evento, Id usuario y cantidad.
2. La Minimal API recibe la petición.
3. FluentValidation valida los datos.
4. MediatR envía el `CreateReservationCommand`.
5. El handler verifica que exista el usuario.
6. Busca el evento mediante su código.
7. Comprueba la disponibilidad.
8. Inicia una transacción.
9. Reduce las entradas disponibles.
10. Crea la reserva.
11. Entity Framework Core persiste los cambios.
12. Si todo funciona, hace `COMMIT`.
13. La API devuelve `201 Created`.

## 4. Concurrencia

La entidad `Event` utiliza `RowVersion` para implementar **concurrencia optimista**.

Esto es importante porque varios compradores pueden intentar reservar entradas del mismo evento simultáneamente.

```text
Cliente A ──────┐
                ├──> Evento
Cliente B ──────┘
```

El control de concurrencia permite detectar cambios realizados por otra operación y evitar actualizaciones inconsistentes.

## 5. Persistencia

La solución utiliza:

- SQL Server.
- Entity Framework Core.
- Repository Pattern.
- Unit of Work.
- Transacciones.

Relaciones principales:

```text
User 1 ─────── N Reservation N ─────── 1 Event
```

Una reserva pertenece a un usuario y a un evento.

El código del evento debe ser único.

## 6. Endpoint principal

El prototipo expone:

```http
POST /api/reservations
Content-Type: application/json
```

Ejemplo:

```json
{
  "eventCode": "EVT-001",
  "userId": "11111111-1111-1111-1111-111111111111",
  "quantity": 2
}
```

Respuesta esperada:

```http
201 Created
```

con los datos de la reserva creada.

## 7. Cliente de consola

El cliente permite probar el flujo completo sin construir un frontend:

```text
Usuario
  │
  ▼
Console
  │
  │ HTTP POST
  ▼
FunEvents API
  │
  ▼
SQL Server
```

## 8. .NET Aspire

La arquitectura de desarrollo es:

```text
                 .NET Aspire
                     │
          ┌──────────┴──────────┐
          ▼                     ▼
   FunEvents API            SQL Server
          │                     │
          └──────────┬──────────┘
                     ▼
                 funevents
```

El recurso de base de datos se referencia desde la API mediante el nombre lógico `funevents`.

## 9. Principios y patrones

La solución está diseñada aplicando principios de diseño y patrones que permiten mantener el código desacoplado, mantenible y preparado para evolucionar a medida que aumenten los canales de venta y los casos de uso de FunEvents

La solución aplica:

- Separation of Concerns.
Cada componente tiene una responsabilidad específica.
  
  API
  
 └── Maneja HTTP y endpoints

Application

 └── Coordina casos de uso

Domain

 └── Contiene entidades y reglas del negocio

Infrastructure

 └── Maneja persistencia y acceso a recursos externos

 
- Dependency Inversion.
Las capas internas dependen de abstracciones y no de implementaciones concretas.
```text
Application
     │
     ▼
IReservationRepository
     ▲
     │
Infrastructure
     │
     ▼
ReservationRepository
```

- Single Responsibility.
De esta manera, un cambio en la validación no obliga a modificar el repositorio o el endpoint.
```tex
CreateReservationCommand
        ↓
Representa la solicitud

CreateReservationValidator
        ↓
Valida la solicitud

CreateReservationHandler
        ↓
Ejecuta el caso de uso

ReservationRepository
        ↓
Persiste la reserva
```
- Clean Architecture.
La regla principal es que las dependencias apuntan hacia el interior, evitando que el dominio dependa de tecnologías externas como Entity Framework Core o ASP.NET Core.

Esto permite que la lógica del negocio permanezca independiente de la infraestructura.
```text
┌─────────────────────┐
│        API          │
├─────────────────────┤
│    Application      │
├─────────────────────┤
│       Domain        │
├─────────────────────┤
│   Infrastructure    │
└─────────────────────┘
```
- CQRS.
  Se separan las operaciones que modifican información de las operaciones que solamente consultan información.
  ```text
  Commands
 ├── CreateReservationCommands
 ├── CreateReservationHandler
```
```text
Queries
 ├── GetReservationQuery
 ├── GetReservationHandler
```
- MediatR
Se utiliza como mecanismo de mediación entre la API y los handlers de Application.

La API no necesita conocer directamente la implementación del caso de uso:
```text
API
 │
 │ Send(command)
 ▼
MediatR
 │
 ▼
CreateReservationHandler 
```
- Repository Pattern.
El acceso a los datos se abstrae mediante repositorios.
```text
Application
    │
    ▼
IReservationRepository
    ▲
    │
Infrastructure
    │
    ▼
ReservationRepository
```
Esto evita que los handlers tengan que trabajar directamente con DbContext.

- Unit of Work.
UnitOfWork permite agrupar varias operaciones relacionadas dentro de una misma operación de persistencia.
```text
Crear Reservation
        ↓
Actualizar Event
       ↓
   UnitOfWork
       ↓
 SaveChanges / Commit
 ```
- Dependency Injection.
Las dependencias se registran mediante el contenedor de Dependency Injection de ASP.NET Core.

- FluentValidation.
Se utiliza para validar los Commands antes de ejecutar los casos de uso.
```text
  CreateReservationCommand
          ↓
       Validator
          ↓
     ┌────┴────┐
     │         │
   Válido    Inválido
     │         │
     ▼         ▼
 Handler    400 Bad Request
```
- Transacciones.
La creación de una reserva implica modificar más de un dato:
```text
Event
 └── Disminuir AvailableTickets

Reservation
 └── Crear nueva reserva

Estas operaciones deben ejecutarse de manera consistente.

BEGIN TRANSACTION
       │
       ├── Actualizar Event
       │
       ├── Crear Reservation
       │
       ├── SaveChanges
       │
       ▼
     COMMIT
```     
Si ocurre un error: ROLLBACK
De esta manera se evita que el sistema quede en un estado inconsistente.
  
- Concurrencia optimista.

Concurrencia optimista

La entidad Event utiliza RowVersion para detectar modificaciones concurrentes.

Esto es especialmente importante en un sistema de venta de entradas, donde varios usuarios pueden intentar reservar entradas del mismo evento simultáneamente.
```text
Cliente A ──────┐
                ▼
             Event
                ▲
Cliente B ──────┘
```
El mecanismo de concurrencia permite detectar si otro proceso modificó el evento antes de guardar los cambios.

Si se detecta un conflicto, la aplicación captura DbUpdateConcurrencyException y devuelve un resultado controlado en lugar de sobrescribir silenciosamente los cambios.

## 10. Arquitectura objetivo para producción

```text
                         ┌──────────────────┐
                         │   Portal Web     │
                         └────────┬─────────┘
                                  │
┌──────────────────┐              │
│ Oficinas físicas │──────────────┤
└──────────────────┘              │
                                  ▼
┌──────────────────┐       ┌───────────────┐
│ Portal partner   │──────>│ FunEvents API │
└──────────────────┘       └───────┬───────┘
                                  │
┌──────────────────┐              │
│ POS partner      │──────────────┤
└──────────────────┘              │
                                  ▼
                         ┌─────────────────┐
                         │ Application     │
                         │ Domain          │
                         │ Infrastructure │
                         └────────┬────────┘
                                  │
                                  ▼
                            ┌───────────┐
                            │ SQL Server│
                            └───────────┘
```

Los colaboradores deberían consumir la API mediante HTTPS y mecanismos de autenticación/autorización apropiados, evitando acceso directo a la base de datos.

## 11. Decisiones técnicas

### Minimal APIs

Se utilizan porque el requerimiento recomienda ASP.NET Core Minimal APIs y son adecuadas para un prototipo pequeño.

### CQRS

Permite separar operaciones de lectura y escritura y facilita la evolución del sistema.

### Repository + Unit of Work

Abstraen la persistencia y permiten agrupar operaciones que deben ejecutarse consistentemente dentro de una transacción.

### SQL Server

Es apropiado para datos transaccionales como usuarios, eventos y reservas, donde consistencia y transacciones son importantes.

### Aspire

Permite orquestar los recursos necesarios durante el desarrollo y facilita configuración y observabilidad.

## 12. Mejoras futuras

Para una versión productiva se podrían incorporar:

- Autenticación y autorización.
- API Gateway.
- Integración con pagos.
- Gestión de tickets.
- Notificaciones.
- Auditoría.
- Versionado de API.
- Pruebas unitarias e integración.

## 13. Flujo resumido

```text
Console / Portal / POS
        │
        ▼
Minimal API
        │
        ▼
FluentValidation
        │
        ▼
MediatR
        │
        ▼
CreateReservationHandler
        │
        ├── UserRepository
        ├── EventRepository
        │
        ▼
Disponibilidad
        │
        ▼
Transaction
        │
        ├── Actualizar Event
        └── Crear Reservation
        │
        ▼
EF Core
        │
        ▼
SQL Server
        │
        ▼
HTTP 201
```

## 14. Nota sobre el prototipo

El objetivo del prototipo es demostrar el flujo mínimo:

```text
Código de evento + Usuario +  cantidad
            │
            ▼
       POST /reservations
            │
            ▼
       Validar solicitud
            │
            ▼
     Verificar disponibilidad
            │
            ▼
        Crear reserva
            │
            ▼
       Persistir en BD
            │
            ▼
        Confirmar reserva
```

La arquitectura permite que este mismo caso de uso sea consumido posteriormente por el portal principal de FunEvents, oficinas físicas y sistemas de colaboradores sin duplicar la lógica de negocio.
