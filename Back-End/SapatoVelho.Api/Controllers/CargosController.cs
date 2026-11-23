using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargosController(AppDbContext db) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cargo>>> Listar() => Ok(await db.Cargos.AsNoTracking().OrderBy(c => c.Id).ToListAsync());
}
