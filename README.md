# FunEvents

Prototipo de un sistema de gestión y reserva de entradas para **FunEvents**, empresa ficticia dedicada a organizar eventos de entretenimiento como conciertos, teatro y otros espectáculos.

## 1. Descripción

FunEvents debe permitir gestionar la venta/reserva de entradas desde diferentes canales:

- Portal web de FunEvents.
- Oficinas de atención al cliente.
- Portales de colaboradores.
- Aplicaciones/POS de colaboradores.

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
- Configuraciones de entidades.
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

1. El cliente envía código de evento, usuario y cantidad.
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

La solución aplica:

- Separation of Concerns.
- Dependency Inversion.
- Single Responsibility.
- Clean Architecture.
- CQRS.
- Repository Pattern.
- Unit of Work.
- Dependency Injection.
- Validación de entrada.
- Transacciones.
- Concurrencia optimista.

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
- Rate limiting.
- Idempotencia para reservas.
- Integración con pagos.
- Gestión de tickets.
- Notificaciones.
- Cache.
- Mensajería/event-driven architecture.
- Observabilidad centralizada.
- Auditoría.
- Escalabilidad horizontal.
- Gestión de partners.
- Versionado de API.
- Pruebas unitarias e integración.
- Estrategias adicionales para evitar sobreventa bajo alta concurrencia.

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
Código de evento + Usuario
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
