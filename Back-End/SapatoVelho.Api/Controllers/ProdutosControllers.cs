using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Listar() => Ok(await db.Produtos.AsNoTracking().OrderBy(p => p.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> BuscarPorId(int id)
    {
        var produto = await db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return produto is null ? NotFound(new { mensagem = "Produto não encontrado." }) : Ok(produto);
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpPost]
    public async Task<ActionResult<Produto>> Criar(Produto produto)
    {
        produto.Id = 0;
        db.Produtos.Add(produto);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(BuscarPorId), new { id = produto.Id }, produto);
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, Produto dados)
    {
        var produto = await db.Produtos.FindAsync(id);
        if (produto is null) return NotFound(new { mensagem = "Produto não encontrado." });
        produto.Nome = dados.Nome; produto.Preco = dados.Preco; produto.Imagem = dados.Imagem; produto.Estoque = dados.Estoque;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var produto = await db.Produtos.FindAsync(id);
        if (produto is null) return NotFound(new { mensagem = "Produto não encontrado." });
        if (await db.ItensPedido.AnyAsync(i => i.ProdutoId == id))
            return Conflict(new { mensagem = "Este produto já aparece em pedidos e não pode ser excluído. Zere o estoque ou desative-o no catálogo." });
        db.Produtos.Remove(produto);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
