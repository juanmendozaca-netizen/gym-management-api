using GymFlow.Data;
using GymFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SociosController : ControllerBase
{
    private readonly GymFlowDbContext _context;

    public SociosController(GymFlowDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Socio>>> GetSocios(
        [FromQuery] string? nombre,
        [FromQuery] string? dni,
        [FromQuery] int? membresiaId)
    {
        IQueryable<Socio> consulta = _context.Socios
            .AsNoTracking()
            .Include(s => s.Membresia);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            consulta = consulta.Where(s =>
                s.Nombres.Contains(nombre) || s.Apellidos.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(dni))
        {
            consulta = consulta.Where(s => s.Dni == dni);
        }

        if (membresiaId.HasValue)
        {
            consulta = consulta.Where(s => s.MembresiaId == membresiaId.Value);
        }

        return Ok(await consulta
            .OrderBy(s => s.Apellidos)
            .ThenBy(s => s.Nombres)
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Socio>> GetSocio(int id)
    {
        var socio = await _context.Socios
            .AsNoTracking()
            .Include(s => s.Membresia)
            .FirstOrDefaultAsync(s => s.SocioId == id);

        return socio is null ? NotFound() : Ok(socio);
    }

    [HttpPost]
    public async Task<ActionResult<Socio>> PostSocio(Socio socio)
    {
        var membresiaExiste = await _context.Membresias
            .AnyAsync(m => m.MembresiaId == socio.MembresiaId);

        if (!membresiaExiste)
        {
            return BadRequest(new { mensaje = "La membresía indicada no existe." });
        }

        var duplicado = await _context.Socios
            .AnyAsync(s => s.Dni == socio.Dni || s.Correo == socio.Correo);

        if (duplicado)
        {
            return Conflict(new { mensaje = "El DNI o correo electrónico ya está registrado." });
        }

        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSocio), new { id = socio.SocioId }, socio);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutSocio(int id, Socio socio)
    {
        if (id != socio.SocioId)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del cuerpo." });
        }

        var existente = await _context.Socios.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var membresiaExiste = await _context.Membresias
            .AnyAsync(m => m.MembresiaId == socio.MembresiaId);

        if (!membresiaExiste)
        {
            return BadRequest(new { mensaje = "La membresía indicada no existe." });
        }

        var duplicado = await _context.Socios
            .AnyAsync(s => (s.Dni == socio.Dni || s.Correo == socio.Correo) && s.SocioId != id);

        if (duplicado)
        {
            return Conflict(new { mensaje = "El DNI o correo electrónico ya está registrado por otro socio." });
        }

        existente.Dni = socio.Dni;
        existente.Nombres = socio.Nombres;
        existente.Apellidos = socio.Apellidos;
        existente.Correo = socio.Correo;
        existente.FechaNacimiento = socio.FechaNacimiento;
        existente.MembresiaId = socio.MembresiaId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSocio(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio is null)
        {
            return NotFound();
        }

        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
