using GymFlow.Data;
using GymFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembresiasController : ControllerBase
{
    private readonly GymFlowDbContext _context;

    public MembresiasController(GymFlowDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Membresia>>> GetMembresias(
        [FromQuery] string? nombre,
        [FromQuery] bool? activa)
    {
        IQueryable<Membresia> consulta = _context.Membresias.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            consulta = consulta.Where(m => m.Nombre.Contains(nombre));
        }

        if (activa.HasValue)
        {
            consulta = consulta.Where(m => m.Activa == activa.Value);
        }

        return Ok(await consulta.OrderBy(m => m.Nombre).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Membresia>> GetMembresia(int id)
    {
        var membresia = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MembresiaId == id);

        return membresia is null ? NotFound() : Ok(membresia);
    }

    [HttpGet("{id:int}/socios")]
    public async Task<ActionResult<IEnumerable<Socio>>> GetSocios(int id)
    {
        var existe = await _context.Membresias
            .AsNoTracking()
            .AnyAsync(m => m.MembresiaId == id);

        if (!existe)
        {
            return NotFound();
        }

        var socios = await _context.Socios
            .AsNoTracking()
            .Where(s => s.MembresiaId == id)
            .OrderBy(s => s.Apellidos)
            .ThenBy(s => s.Nombres)
            .ToListAsync();

        return Ok(socios);
    }

    [HttpPost]
    public async Task<ActionResult<Membresia>> PostMembresia(Membresia membresia)
    {
        var nombreExiste = await _context.Membresias
            .AnyAsync(m => m.Nombre == membresia.Nombre);

        if (nombreExiste)
        {
            return Conflict(new { mensaje = "Ya existe una membresía con ese nombre." });
        }

        _context.Membresias.Add(membresia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMembresia), new { id = membresia.MembresiaId }, membresia);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutMembresia(int id, Membresia membresia)
    {
        if (id != membresia.MembresiaId)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del cuerpo." });
        }

        var existente = await _context.Membresias.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        var nombreExiste = await _context.Membresias
            .AnyAsync(m => m.Nombre == membresia.Nombre && m.MembresiaId != id);

        if (nombreExiste)
        {
            return Conflict(new { mensaje = "Ya existe otra membresía con ese nombre." });
        }

        existente.Nombre = membresia.Nombre;
        existente.Descripcion = membresia.Descripcion;
        existente.DuracionMeses = membresia.DuracionMeses;
        existente.Precio = membresia.Precio;
        existente.Activa = membresia.Activa;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMembresia(int id)
    {
        var membresia = await _context.Membresias.FindAsync(id);
        if (membresia is null)
        {
            return NotFound();
        }

        var tieneSocios = await _context.Socios
            .AnyAsync(s => s.MembresiaId == id);

        if (tieneSocios)
        {
            return Conflict(new { mensaje = "No se puede eliminar una membresía que tiene socios asociados." });
        }

        _context.Membresias.Remove(membresia);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
