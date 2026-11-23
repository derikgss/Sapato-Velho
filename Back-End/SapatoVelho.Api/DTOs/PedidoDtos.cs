using System.ComponentModel.DataAnnotations;

namespace SapatoVelho.Api.DTOs;

public sealed class CriarPedidoDto
{
    [Required, StringLength(100)] public string FormaPagamento { get; set; } = "";
    [Required, StringLength(200, MinimumLength = 5)] public string EnderecoEntrega { get; set; } = "";
    [Required, MinLength(1)] public List<ItemPedidoDto> Itens { get; set; } = [];
}

public sealed class ItemPedidoDto
{
    [Range(1, int.MaxValue)] public int ProdutoId { get; set; }
    [Range(1, 100)] public int Quantidade { get; set; }
}
