# AGENTS.md — reglas para trabajar en este backend

Aplican a cualquier persona o agente que modifique este repo. Si una regla estorba, no la rompas en silencio:
dilo y proponlo. Mapa del sistema y plan de liberación: `../docs/` (fuera de git). Reglas del frontend: `../mechanic-app-frontend-/AGENTS.md`.

## Stack (no agregar alternativas sin preguntar)
.NET 9 · ASP.NET Core · HotChocolate 15 (GraphQL, proyecciones, filtros, orden) · EF Core 9 + SQL Server · JWT Bearer ·
Azure Blob Storage (fotos) · xUnit (`WebApi.Tests`).

## 1. Base de datos: el dueño es la única persona que la toca
- **Ningún agente se conecta a la base ni ejecuta SQL** (ni `dotnet ef database update`, ni scripts, ni consultas "de lectura").
- El esquema no usa migraciones de EF: vive en scripts SQL (`../mechanic-app-frontend-/querys.sql`, `setup_work_orders.sql`).
  Si un cambio necesita columna, índice o trigger nuevo, **escribe el SQL propuesto en el PR/doc** y el dueño lo ejecuta.
- La API no modifica el esquema. Los modelos (`Models/*.cs`) reflejan la base: nulabilidad igual a la columna
  (si la columna acepta `NULL`, la propiedad es nullable).
- La base ya aplica reglas (CHECK, índice único de orden abierta, triggers de stock y de orden cerrada). **No las dupliques**
  en C#; tradúcelas en `GraphQL/DbErrorFilter.cs` (número de error SQL o nombre del constraint → `extensions.code` + mensaje en español).

## 2. Secretos y configuración
- Nunca hagas commit de cadenas de conexión, llaves JWT ni llaves de Azure. Desarrollo: `dotnet user-secrets`; producción: variables de entorno.
  `appsettings*.json` solo lleva valores no sensibles.
- Lo que es solo de desarrollo (`IncludeExceptionDetails`, `EnableSensitiveDataLogging`, Swagger, CORS de `localhost`) va condicionado a
  `app.Environment.IsDevelopment()`. Nunca se devuelve `stackTrace` ni SQL al cliente.

## 3. Autorización
- Toda query y mutation de negocio lleva `[Authorize]` (HotChocolate.AspNetCore.Authorization ya está instalado). Solo `api/auth/login` es público.
- El usuario sale del JWT (`userId`); nunca se acepta `createdBy` ni el rol como argumento del cliente.

## 4. GraphQL: cómo se escribe una operación
- Una clase por tema en `GraphQL/Mutations/<Tema>Mutations.cs` con `[ExtendObjectType(typeof(Mutation))]`, registrada en `Program.cs`.
  Las queries van en `GraphQL/Query.cs` con `[UsePaging]`, `[UseProjection]`, `[UseFiltering]`, `[UseSorting]` cuando son listas.
- Nombres de mutations del dominio de órdenes en español y en infinitivo (`crearOrden`, `agregarRepuestoOrden`); no renombres las existentes:
  el frontend depende de ellas.
- Valida argumentos al inicio y lanza `GraphQLException` con mensaje en español para el usuario final.
- Escrituras que tocan varias tablas o totales: **una transacción** (`BeginTransactionAsync`) y devolver la entidad recalculada
  (patrón de `WorkOrderItemMutations` + `WorkOrderSupport`).
- Reglas puras (transiciones de estado, totales, tipos válidos, zona horaria) van en `Services/BusinessRules.cs`, sin EF, y con test en `WebApi.Tests`.
- Evita N+1: campos calculados por lista con DataLoader (ver `WorkOrderFields.cs` / `AssignedMechanicsDataLoader`).

## 5. Reglas de negocio
- Estados: `Recibido → En proceso → Listo → Entregado`; `Cancelado` desde los tres primeros. `Entregado` y `Cancelado` no admiten cambios.
- Un vehículo, una orden abierta. El stock solo cambia por movimientos (`registrarMovimiento`) o repuestos de una orden (trigger); los
  movimientos son inmutables. Nada escribe `stock_quantity` directo.
- Los totales de la orden se calculan aquí (`WorkOrderRules.Totals`) en la misma transacción que el cambio.
- Fotos de una orden `Entregado` son evidencia: no se editan ni borran.
- Fechas: zona del taller `WorkOrderRules.TimeZoneId` (`America/Managua`). Si se cambia cómo se emiten las fechas, avisa: el frontend
  las interpreta en un solo lugar (`shared/utils/dates.ts`).

## 6. Contrato con el frontend
El contrato está en `../mechanic-app-frontend-/docs/work-orders-api.md` (y `work-orders-database.md`). Si agregas, cambias o pruebas una
operación, **actualiza ese doc en el mismo cambio** (y quita la ⚠️ cuando quede probada). Un cambio que rompe el contrato se coordina con el frontend.

## 7. Cómo trabajar un cambio
1. Lee este archivo, el doc de API y busca algo parecido ya hecho.
2. Cambios pequeños y enfocados; no reorganices código viejo (farmacia) en el mismo cambio.
3. Antes de dar algo por terminado: `dotnet build` y `dotnet test` pasan. Lo que toque la base lo prueba el dueño.
4. No hagas commit de secretos. No ejecutes SQL.
5. Si algo es ambiguo (un campo, una regla), pregunta antes de inventarlo.
