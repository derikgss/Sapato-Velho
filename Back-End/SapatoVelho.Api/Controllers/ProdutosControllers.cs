using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/produtos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Listar()
    {
        var produtos = await _context.Produtos
            .AsNoTracking()
            .ToListAsync();

        return Ok(produtos);
    }

    // GET: api/produtos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> BuscarPorId(int id)
    {
        var produto = await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        return Ok(produto);
    }

    // POST: api/produtos
    [HttpPost]
    public async Task<ActionResult<Produto>> Criar(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = produto.Id },
            produto);
    }

    // PUT: api/produtos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest(new { mensagem = "O ID da URL não corresponde ao ID do produto." });

        var produtoExiste = await _context.Produtos.AnyAsync(p => p.Id == id);

        if (!produtoExiste)
            return NotFound(new { mensagem = "Produto não encontrado." });

        _context.Entry(produto).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/produtos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto is null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
