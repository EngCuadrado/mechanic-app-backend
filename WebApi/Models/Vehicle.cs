using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("vehicle_id")]
    public int vehicleId { get; set; }




}


/*
CREATE TABLE [vehicles] (
[vehicle_id] INT IDENTITY(1,1) PRIMARY KEY,
[company_id] int, -- Empresa que actualmente tiene el vehículo. Null = Disponible
[plate] nvarchar(255) UNIQUE, -- Placa
[vin] nvarchar(255) UNIQUE, -- Número de serie del vehículo
[chassis_number] nvarchar(255) UNIQUE, -- Número de chasis
[brand] nvarchar(255), -- Marca
[model] nvarchar(255), -- Modelo
[year] int, -- Año de fabricación
[current_mileage] int, -- Kilometraje actual
[next_maintenance_date] date, -- Fecha del proximo mantenimiento
[next_maintenance_mileage] int, -- Kilometraje del proximo mantenimiento
[insurance_expiration_date] date, -- Fecha de vencimiento del seguro
[mechanical_inspection_expiration] date, -- Fecha de vencimiento de la inspección mecánica
[emissions_inspection_expiration] date, -- Fecha de vencimiento de la inspección de emisiones
[status] nvarchar(255) -- Estado del vehículo  [en mantenimiento, disponible, alquilado, etc.] 
)
GO
*/