# Base de datos de órdenes de trabajo (lo que usa la API)

- `work_orders`: `entry_timestamp`, `exit_timestamp`, `current_status`, `vehicle_id`, … `status_changed_at DATETIME2 NULL` guarda el último cambio de estado (ya aplicada en la BD). `created_by` (FK a users) guarda quién creó la orden.
- `work_order_services.mechanic_id` (nullable): el mecánico se asigna por servicio; `assignedMechanics` los agrupa por orden.
- La API no modifica el esquema.

## Referencia del ALTER ejecutado
Para medir `stalled` y mostrar "hace 3 h" desde el último cambio de estado:
```sql
ALTER TABLE work_orders ADD status_changed_at DATETIME2 NULL;
CREATE INDEX IX_work_orders_status_entry ON work_orders (current_status, entry_timestamp);
```
La API ya la mapea y la usa con `COALESCE(status_changed_at, entry_timestamp)` en `stalled`. El índice también acelera `workOrderStatusCounts` y el dashboard.
