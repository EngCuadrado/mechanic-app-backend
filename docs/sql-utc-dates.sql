-- Lo ejecuta quien administra la BD. La API NO corre este script.
-- Desde el 2026-11-22 el backend guarda todas las fechas en UTC (WorkOrderRules.Now() = DateTime.UtcNow).
-- Antes, crearOrden / cambiarEstadoOrden / cancelarOrden guardaban entry_timestamp, exit_timestamp y status_changed_at
-- en hora de Managua (UTC-6), y la API las devolvía con "Z": en pantalla salían 6 h corridas.
--
-- Este script pasa a UTC las órdenes que se guardaron en hora de Managua. NO es idempotente: correrlo dos veces suma 12 h.
-- 1) Pon en @desde la fecha en que se publicó el backend que escribía en hora de Managua, y en @hasta la fecha en que se
--    publicó el de UTC. Las órdenes fuera de ese rango no se tocan.
-- 2) Revisa el SELECT de vista previa antes de confirmar (cambia ROLLBACK por COMMIT al final).
-- Las demás tablas (inspecciones, fotos, movimientos, alquileres) usaban GETDATE() o DateTime.Now del servidor; si el servidor
-- de producción corre en UTC (lo normal en Azure), ya están en UTC y no hay que tocarlas.

DECLARE @desde DATETIME = NULL; -- ej. '2026-11-08T00:00:00'
DECLARE @hasta DATETIME = NULL; -- ej. '2026-11-23T00:00:00'
IF @desde IS NULL OR @hasta IS NULL BEGIN RAISERROR(N'Define @desde y @hasta antes de correr el script.', 16, 1); RETURN; END;

SET XACT_ABORT ON;
BEGIN TRAN;

SELECT work_order_id, entry_timestamp AS antes, DATEADD(HOUR, 6, entry_timestamp) AS despues
FROM work_orders WHERE entry_timestamp >= @desde AND entry_timestamp < @hasta;

UPDATE work_orders
SET entry_timestamp = DATEADD(HOUR, 6, entry_timestamp),
    exit_timestamp = DATEADD(HOUR, 6, exit_timestamp),
    status_changed_at = DATEADD(HOUR, 6, status_changed_at)
WHERE entry_timestamp >= @desde AND entry_timestamp < @hasta;

ROLLBACK; -- cambiar por COMMIT cuando la vista previa esté bien
