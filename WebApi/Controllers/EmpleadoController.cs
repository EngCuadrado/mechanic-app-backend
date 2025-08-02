using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Empleados;
using WebApi.Models.Empleados.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadoController : ControllerBase
    {
        private readonly AppDbContext _dataBase;

        public EmpleadoController(AppDbContext context)
        {
            _dataBase = context;
        }
        
        [HttpPost("createCargo/")]
        public async Task<ActionResult<ApiResponse>> CrearCargo([FromBody] CargoDto cargoDto)
        {
            if (string.IsNullOrWhiteSpace(cargoDto.cargo))
            {
                return BadRequest(new ApiResponse
                {
                    success = false,
                    message = "El nombre del cargo es obligatorio"
                });
            }

            // Validar si ya existe un cargo con el mismo nombre (ignorando mayúsculas/minúsculas)
            bool cargoExistente = await _dataBase.Cargos
                .AnyAsync(c => c.cargo.ToLower() == cargoDto.cargo.ToLower());

            if (cargoExistente)
            {
                return Conflict(new ApiResponse
                {
                    success = false,
                    message = "El cargo ya existe"
                });
            }

            var cargo = new Cargo
            {
                cargo = cargoDto.cargo
            };

            try
            {
                _dataBase.Cargos.Add(cargo);
                await _dataBase.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    success = true,
                    message = "Cargo agregado"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    success = false,
                    message = $"Ocurrió un error al guardar el cargo: {ex.Message}"
                });
            }
        }


        [HttpGet("getAllCargos/")]
        public async Task<ActionResult<ApiResponse>> GetAllCargos()
        {
            try
            {
                var cargos = await _dataBase.Cargos.ToListAsync();

                return Ok(new ApiResponse
                {
                    success = true,
                    data = cargos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    success = false,
                    message = $"Error al obtener los cargos: {ex.Message}"
                });
            }
        }



    
    }
    
}

