using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SapatoVelho.Api.Models;

[Table("Produtos")]
public class Produto
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("NomeProduto", TypeName = "varchar(100)")]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(100)]
    [Column("Imagem", TypeName = "varchar(100)")]
    public string Imagem { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Estoque { get; set; }
}
