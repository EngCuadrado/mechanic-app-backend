using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("getAll/")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            ApiResponse response = new ApiResponse();

            if (product == null)
            {
                response.success = false;
                response.message = "No se encontró el producto";
                return NotFound(response);  // Respuesta 404 con el mensaje
            }

            response.success = true;
            response.data = product;
            return Ok(response);  // Respuesta 200 con el producto
        }


        [HttpPost("get")]
        public async Task<ActionResult<ApiResponse>> PostProduct(Product product)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                // Intentar agregar el producto a la base de datos
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Si la inserción es exitosa
                apiResponse.success = true;
                apiResponse.message = "Elemento añadido exitosamente";
                return Ok(apiResponse);  // Respuesta 200 OK con los detalles
            }
            catch (Exception ex)
            {
                // En caso de error, se puede devolver un mensaje de error y el detalle de la excepción
                apiResponse.success = false;
                apiResponse.message = $"Error al añadir el producto: {ex.Message}";
                return StatusCode(500, apiResponse);// Respuesta 500 con el mensaje de error
            }
        }


        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);
    }
}
