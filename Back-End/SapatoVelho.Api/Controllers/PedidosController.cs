using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.DTOs;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/pedidos")]
public class PedidosController(AppDbContext db) : ControllerBase
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private static object Resumo(Pedido p) => new { p.Id, p.DataPedido, p.ValorPedido, formaPagamento = p.FormaPagamento, p.EnderecoEntrega, p.Status,
        itens = p.Itens.Select(i => new { produtoId = i.ProdutoId, nome = i.Produto!.Nome, quantidade = i.Quantidade, precoUnidade = i.PrecoUnidade, subtotal = i.PrecoUnidade * i.Quantidade }) };

    [HttpPost]
    public async Task<IActionResult> Criar(CriarPedidoDto dto)
    {
        var agregados = dto.Itens.GroupBy(i => i.ProdutoId).Select(g => new { ProdutoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) }).ToList();
        if (agregados.Any(i => i.Quantidade < 1 || i.Quantidade > 100)) return BadRequest(new { mensagem = "Quantidade inválida." });
        await using var tx = await db.Database.BeginTransactionAsync();
        var ids = agregados.Select(i => i.ProdutoId).ToList();
        var produtos = await db.Produtos.Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        if (produtos.Count != ids.Count) return BadRequest(new { mensagem = "Um ou mais produtos não existem mais no catálogo." });
        foreach (var item in agregados)
            if (produtos[item.ProdutoId].Estoque < item.Quantidade)
                return Conflict(new { mensagem = $"Estoque insuficiente para {produtos[item.ProdutoId].Nome}." });

        var pedido = new Pedido { UsuarioId = UsuarioId, DataPedido = DateTime.UtcNow, FormaPagamento = dto.FormaPagamento.Trim(), EnderecoEntrega = dto.EnderecoEntrega.Trim(), Status = "Recebido" };
        foreach (var item in agregados)
        {
            var produto = produtos[item.ProdutoId];
            pedido.Itens.Add(new ItemPedido { ProdutoId = produto.Id, Quantidade = item.Quantidade, PrecoUnidade = produto.Preco });
            produto.Estoque -= item.Quantidade;
            pedido.ValorPedido += produto.Preco * item.Quantidade;
        }
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        await db.Entry(pedido).Collection(p => p.Itens).Query().Include(i => i.Produto).LoadAsync();
        return CreatedAtAction(nameof(BuscarPorId), new { id = pedido.Id }, Resumo(pedido));
    }

    [HttpGet("meus")]
    public async Task<IActionResult> Meus()
    {
        var pedidos = await db.Pedidos.Where(p => p.UsuarioId == UsuarioId).Include(p => p.Itens).ThenInclude(i => i.Produto).AsNoTracking().OrderByDescending(p => p.DataPedido).ToListAsync();
        return Ok(pedidos.Select(Resumo));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var pedido = await db.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto).AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (pedido is null) return NotFound(new { mensagem = "Pedido não encontrado." });
        if (pedido.UsuarioId != UsuarioId && !User.IsInRole("Admin") && !User.IsInRole("Funcionario")) return Forbid();
        return Ok(Resumo(pedido));
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusPedidoDto dto)
    {
        var permitidos = new[] { "Recebido", "Pago", "Enviado", "Entregue", "Cancelado" };
        if (!permitidos.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { mensagem = "Status inválido." });
        var pedido = await db.Pedidos.FindAsync(id);
        if (pedido is null) return NotFound(new { mensagem = "Pedido não encontrado." });
        pedido.Status = permitidos.First(s => s.Equals(dto.Status, StringComparison.OrdinalIgnoreCase));
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok((await db.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto).AsNoTracking().OrderByDescending(p => p.DataPedido).ToListAsync()).Select(Resumo));
}
