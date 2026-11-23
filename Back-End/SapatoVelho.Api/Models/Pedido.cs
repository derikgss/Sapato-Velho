using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SapatoVelho.Api.Models;

[Table("Pedidos")]
public class Pedido
{
    [Key, Column("ID")] public int Id { get; set; }
    [Column("UsuariosID")] public int UsuarioId { get; set; }
    [Column("DataPedido")] public DateTime DataPedido { get; set; }
    [Column("ValorPedido", TypeName = "decimal(10,2)")] public decimal ValorPedido { get; set; }
    [Required, StringLength(100), Column("FormaPgt", TypeName = "varchar(100)")] public string FormaPagamento { get; set; } = "";
    [Required, StringLength(200), Column("EnderecoEntrega", TypeName = "varchar(200)")] public string EnderecoEntrega { get; set; } = "";
    [Required, StringLength(30), Column(TypeName = "varchar(30)")] public string Status { get; set; } = "Recebido";
    public User? Usuario { get; set; }
    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}

[Table("ItensPedido")]
public class ItemPedido
{
    [Key, Column("ID")] public int Id { get; set; }
    [Column("PedidosID")] public int PedidoId { get; set; }
    [Column("ProdutosID")] public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal PrecoUnidade { get; set; }
    public Pedido? Pedido { get; set; }
    public Produto? Produto { get; set; }
}
