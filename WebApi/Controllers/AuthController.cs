using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.DTOs;
using WebApi.Models.Empleados;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dataBase;
        private readonly JwtOptions _jwt;

        private readonly BlobStorageService _blobStorageService;

        private readonly IConfiguration _IConfiguration;


        public AuthController(AppDbContext db, IOptions<JwtOptions> jwtOptions, BlobStorageService blobStorageService, IConfiguration configuration)
        {
            _dataBase = db;
            _jwt = jwtOptions.Value;
            _blobStorageService = blobStorageService;
            _IConfiguration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.email) || string.IsNullOrWhiteSpace(dto?.password))
                return BadRequest(new ApiResponse { success = false, message = "Usuario y contraseña son obligatorios" });

            var user = await _dataBase.Users.Where(u => u.email == dto.email).FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized(new ApiResponse { success = false, message = "Credenciales inválidas" });

            // 1. Convertimos la contraseña plana a bytes
            var passwordBytes = Encoding.UTF8.GetBytes(dto.password);

            // 2. Calculamos el hash SHA-256
            var hashBytes = SHA256.HashData(passwordBytes);

            // 3. Lo convertimos a string (hexadecimal). 
            // Nota: ToLower() es opcional, depende de si en tu BD guardaste el hash en mayúsculas o minúsculas.
            var hashedInput = Convert.ToHexString(hashBytes).ToLower();

            // 4. Comparamos los hashes
            var okPassword = user.passwordHash == hashedInput;

            //var okPassword = user.passwordHash == dto.password;
            if (!okPassword)
                return Unauthorized(new ApiResponse { success = false, message = "Credenciales inválidas" });

            if (!user.isActive)
                return Forbid(); // o Unauthorized con mensaje

            var (tokenString, expires) = GenerateJwt(user);

            var resp = new
            {
                token = tokenString,
                expiresAt = expires,
                userId = user.userId,
                names = user.name,
                lastnames = user.lastName,
                email = user.email,
                roleName = user.role
            };

            return Ok(new ApiResponse { success = true, data = resp });
        }

        private (string token, DateTime expires) GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_jwt.ExpireMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.email ?? ""),
                new Claim("userId", user.userId.ToString()),
                //new Claim("username", emp.user ?? ""),
                new Claim("names", user.name ?? ""),
                new Claim("lastnames", user.lastName ?? ""),
                new Claim("email", user.email ?? ""),
                //new Claim("phone", emp.phone ?? ""),
                //new Claim("email", emp.email ?? ""),
                //new Claim("urlPhoto", emp.url_photo ?? ""),
                new Claim("roleId", user.role),
                new Claim("role", user.role ?? ""),
                new Claim("roleName", user.role ?? "")
                //new Claim("statusId", emp.EmployeeStatusId.ToString()), 
                //new Claim("statusName", emp.EmployeeStatus?.name ?? ""),
                //new Claim("status", emp.status ?? ""),
                //new Claim("hiringDate", emp.hiring_date.ToString("yyyy-MM-dd"))
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (tokenString, expires);
        }

    


        [HttpPost]
        [Route("uploadImage")]
        public async Task<ActionResult<string>> OdometerUpload(BlobRequestData request)
        {
            if (request.Picture?.FileName == null || request.Picture.Length == 0)
            {
                return BadRequest(new { success = false });
            }

            try
            {
                string? base64String = null;
                string? base64Extension = System.IO.Path.GetExtension(request.Picture.FileName);
                if (!string.IsNullOrEmpty(base64Extension))
                    base64Extension = base64Extension.Replace(".", "").ToLower();

                using (var memoryStream64 = new MemoryStream())
                {
                    // Copia el contenido del stream del archivo al MemoryStream de forma síncrona
                    // file.OpenReadStream() abre el stream del archivo subido
                    request.Picture.CopyTo(memoryStream64);

                    // Convierte los bytes en el MemoryStream a un array de bytes
                    byte[] fileBytes = memoryStream64.ToArray();

                    // Convierte el array de bytes a una cadena Base64
                    base64String = Convert.ToBase64String(fileBytes);
                }

                if (string.IsNullOrEmpty(base64String))
                {
                    return BadRequest(new { success = false, error = "El archivo está vacio!", odometer = 0 });
                }

                using var fileStream = request.Picture.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    await _blobStorageService.UploadImageAsync(memoryStream, request.blobName, "photos");

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    byte[] byteArray = memoryStream.ToArray();

                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false });
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones y registrar detalles
                //_logger.LogError(ex, "Error procesando la imagen");
                return BadRequest(new { success = false });
            }
        }

    }
}