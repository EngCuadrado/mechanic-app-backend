-- Lo ejecuta quien administra la BD. La API NO corre este script. Idempotente (busca por nombre).
-- Checklist básico (2026-11-22): recibir y entregar solo piden 4 cosas que cualquiera revisa al llegar el carro.
-- El resto del checklist queda INACTIVO (no se borra: las inspecciones viejas siguen mostrando sus respuestas).
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM inspection_categories WHERE name = N'Revisión básica')
    INSERT inspection_categories (name, sort_order, is_active) VALUES (N'Revisión básica', 0, 1);
DECLARE @cat INT = (SELECT TOP 1 category_id FROM inspection_categories WHERE name = N'Revisión básica' ORDER BY category_id);

-- 1) Combustible como opciones (antes un número en %): ítem nuevo para no mezclar respuestas viejas en %.
IF NOT EXISTS (SELECT 1 FROM inspection_checklist_items WHERE name = N'Nivel de combustible' AND input_type = 'select')
    INSERT inspection_checklist_items (name, input_type, options, category_id, applies_to, is_required, is_active)
    VALUES (N'Nivel de combustible', 'select', N'["Vacío","1/4","1/2","3/4","Lleno"]', @cat, 'Ambas', 1, 1);

-- 2-4) Sí/no que ya existían: se mueven a la categoría básica (la llanta incluye ahora gata y herramientas).
UPDATE inspection_checklist_items SET name = N'Llanta de repuesto, gata y herramientas'
WHERE name = N'Llanta de repuesto presente';
UPDATE inspection_checklist_items SET name = N'Documentos del carro'
WHERE name = N'Tarjeta de circulación / documentos';

UPDATE inspection_checklist_items SET category_id = @cat, is_required = 1, is_active = 1, applies_to = 'Ambas'
WHERE name IN (N'Nivel de combustible', N'Llanta de repuesto, gata y herramientas', N'Tablero sin luces de advertencia', N'Documentos del carro');

-- Todo lo demás, inactivo.
UPDATE inspection_checklist_items SET is_active = 0 WHERE category_id IS NULL OR category_id <> @cat;

SELECT i.item_id, i.name, i.input_type, i.is_required FROM inspection_checklist_items i WHERE i.is_active = 1 ORDER BY i.item_id;
COMMIT;
