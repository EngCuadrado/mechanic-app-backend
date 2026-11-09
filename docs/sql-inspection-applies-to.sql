-- Lo ejecuta quien administra la BD. Idempotente. La API NO corre este script.
-- 1) Columna: los ítems existentes quedan en 'Ambas' (no cambia nada de lo actual).
IF COL_LENGTH('inspection_checklist_items', 'applies_to') IS NULL
    ALTER TABLE inspection_checklist_items
        ADD applies_to VARCHAR(10) NOT NULL
            CONSTRAINT DF_inspection_items_applies_to DEFAULT 'Ambas'
            CONSTRAINT CK_inspection_items_applies_to CHECK (applies_to IN ('Entrada', 'Salida', 'Ambas'));
GO

-- 2) Seed propuesto (ajustar con el taller). Categorías nuevas solo si no existen por nombre.
DECLARE @cats TABLE (name NVARCHAR(255), sort_order INT);
INSERT @cats VALUES (N'General', 1), (N'Exterior', 2), (N'Interior', 3), (N'Trabajo', 4), (N'Cierre', 5);
INSERT inspection_categories (name, sort_order, is_active)
SELECT c.name, c.sort_order, 1 FROM @cats c
WHERE NOT EXISTS (SELECT 1 FROM inspection_categories x WHERE x.name = c.name);

DECLARE @items TABLE (cat NVARCHAR(255), name NVARCHAR(255), input_type VARCHAR(20), options NVARCHAR(MAX), applies_to VARCHAR(10), is_required BIT);
INSERT @items VALUES
 (N'General',  N'Kilometraje',                              'number',   NULL, 'Entrada', 0),
 (N'General',  N'Nivel de combustible',                     'select',   N'["Vacío","1/4","1/2","3/4","Lleno"]', 'Entrada', 0),
 (N'General',  N'Objetos de valor dentro del carro',        'textarea', NULL, 'Entrada', 0),
 (N'Exterior', N'Golpes o rayones visibles',                'textarea', NULL, 'Entrada', 0),
 (N'Exterior', N'Estado de las llantas',                    'select',   N'["Bueno","Regular","Malo"]', 'Entrada', 0),
 (N'Exterior', N'Luces funcionan',                          'checkbox', NULL, 'Entrada', 0),
 (N'Interior', N'Llanta de repuesto, gata y herramientas',  'checkbox', NULL, 'Entrada', 0),
 (N'Trabajo',  N'El problema reportado quedó resuelto',     'checkbox', NULL, 'Salida',  1),
 (N'Trabajo',  N'Prueba de manejo realizada',               'checkbox', NULL, 'Salida',  0),
 (N'Trabajo',  N'Sin fugas',                                'checkbox', NULL, 'Salida',  0),
 (N'Cierre',   N'Kilometraje de salida',                    'number',   NULL, 'Salida',  0),
 (N'Cierre',   N'Carro limpio, sin herramientas olvidadas', 'checkbox', NULL, 'Salida',  0);

INSERT inspection_checklist_items (name, input_type, options, category_id, applies_to, is_required, is_active)
SELECT i.name, i.input_type, i.options, (SELECT TOP 1 category_id FROM inspection_categories WHERE name = i.cat ORDER BY category_id),
       i.applies_to, i.is_required, 1
FROM @items i
WHERE NOT EXISTS (SELECT 1 FROM inspection_checklist_items x WHERE x.name = i.name AND x.applies_to = i.applies_to);
