# API de órdenes de trabajo (GraphQL)

Probado = ejecutado contra la BD real (solo lecturas). No probado = compila pero no se ejecutó.

## Reglas de fechas
- `entryTimestamp` / `exitTimestamp` se escriben en hora local de `America/Panama` (UTC-5, sin offset) con `WorkOrderRules.Now()`.
- "Hoy" (`createdToday`) = día calendario en esa misma zona. Zona única: `WorkOrderRules.TimeZoneId` (`Services/BusinessRules.cs`).
- Las órdenes creadas antes de este cambio usaban `DateTime.Now` del servidor; si el servidor no estaba en esa zona, sus fechas pueden estar desfasadas.
- Otras tablas (facturas, inspecciones, movimientos) siguen con `DateTime.Now` del servidor.

## Queries nuevas
### workOrderStatusCounts(entryFrom: DateTime, entryTo: DateTime, vehicleId: Int): [WorkOrderStatusCount!]!
Una fila por estado (incluye 0). `GROUP BY current_status` en la BD. `DateTime` exige formato ISO con zona (`2020-01-01T00:00:00Z`). Probado.
```graphql
{ workOrderStatusCounts { status count } }
```
```json
{"data":{"workOrderStatusCounts":[{"status":"Recibido","count":1},{"status":"En proceso","count":0},{"status":"Listo","count":0},{"status":"Entregado","count":3},{"status":"Cancelado","count":1}]}}
```
Con filtros (`entryFrom`, `vehicleId`) probado: devuelve las 5 filas en 0 cuando no hay coincidencias.

### workOrderDashboard(stalledHours: Int = 24): WorkOrderDashboard!
Campos: `createdToday`, `readyToDeliver`, `stalled`. Probado.
```graphql
{ workOrderDashboard { createdToday readyToDeliver stalled } }
```
```json
{"data":{"workOrderDashboard":{"createdToday":0,"readyToDeliver":0,"stalled":0}}}
```
`stalledHours: 1` devolvió `stalled: 1`. `stalledHours <= 0` lanza error.
`stalled` = Recibido/En proceso con más de N horas desde `statusChangedAt` (usa `entryTimestamp` si es null, como en las órdenes anteriores a la columna).

## Campo nuevo en WorkOrder
### assignedMechanics: [Mechanics!]!
Mecánicos distintos de los servicios de la orden, cargados en lote con `AssignedMechanicsDataLoader` (una consulta por nivel, sin N+1). Funciona dentro de `workOrders(first:…)` con filtros/orden.
```graphql
{ workOrders(first: 5) { nodes { workOrderId assignedMechanics { mechanicId firstName lastName } } } }
```
Probado solo con listas vacías (ninguna orden real tiene mecánico asignado). No probado con mecánicos reales.

### statusChangedAt: DateTime
Fecha del último cambio de estado. Se escribe en `crearOrden`, `cambiarEstadoOrden` y `cancelarOrden`. Lectura probada (null en órdenes antiguas); la escritura no se probó (requiere crear/mover órdenes reales).

## Inspecciones: checklist por tipo (Entrada / Salida)
**Requiere la columna `applies_to`**: ejecutar `docs/sql-inspection-applies-to.sql` (columna con default `Ambas` + seed) antes de levantar esta versión; sin ella las queries de ítems fallan.

Argumentos confirmados contra el schema real (introspección):
- `inspectionForm(vehicleInspectionId: Int, inspectionType: String)`: con id usa el tipo de esa inspección; sin id usa `inspectionType` (`Entrada`/`Salida`) y devuelve ítems de ese tipo y de `Ambas`; sin ninguno devuelve todos. Un tipo inválido da error.
- `vehicleInspection(vehicleInspectionId: Int!)`
- `crearInspeccion(workOrderId: Int!, inspectionType: String!, mileage: Int, inspectionNotes: String, damageMapUrl: String)`: el kilometraje es número (`Int`) y el croquis es URL opcional.
- `editarInspeccion(vehicleInspectionId: Int!, mileage: Int, inspectionNotes: String, damageMapUrl: String)`: omitir conserva, `null` borra.
- `guardarResultadosInspeccion(vehicleInspectionId: Int!, results: [InspectionResultInput!]!, validarCompletitud: Boolean = false)`: un `itemId` que no aplica al tipo de la inspección falla con `extensions.code = "ITEM_NOT_APPLICABLE"`; `validarCompletitud` exige solo los obligatorios que aplican al tipo.
- `addInspectionChecklistItem(..., appliesTo: String = "Ambas")` y `updateInspectionChecklistItem(..., appliesTo: String)`; `InspectionChecklistItem.appliesTo` en el tipo.

Estado: compila y hay prueba unitaria de la regla `Applies`. Nada de esto se ejecutó contra la BD (falta la columna).

## Pendientes
- Unificar `DateTime.Now` del resto de mutaciones con `WorkOrderRules.Now()`.
