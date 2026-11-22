using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CargosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/cargos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cargo>>> Listar()
    {
        var cargos = await _context.Cargos
            .AsNoTracking()
            .ToListAsync();

        return Ok(cargos);
    }
}
