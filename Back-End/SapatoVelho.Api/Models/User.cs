using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SapatoVelho.Api.Models;

[Table("Usuarios")]
public class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Column("Nome", TypeName = "varchar(150)")]
    public string Nome { get; set; } = string.Empty;

    [Column("DataNascimento", TypeName = "date")]
    public DateTime? DataNascimento { get; set; }

    [Required]
    [StringLength(20)]
    [Column("CPF", TypeName = "varchar(20)")]
    public string CPF { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    [Column("Email", TypeName = "varchar(100)")]
    public string Email { get; set; } = string.Empty;

    // Armazene aqui o HASH da senha, nunca a senha em texto puro.
    [Required]
    [StringLength(255)]
    [Column("Senha", TypeName = "varchar(255)")]
    public string Senha { get; set; } = string.Empty;

    [StringLength(200)]
    [Column("Endereco", TypeName = "varchar(200)")]
    public string? Endereco { get; set; }

    [StringLength(20)]
    [Column("CEP", TypeName = "varchar(20)")]
    public string? CEP { get; set; }

    [StringLength(100)]
    [Column("Cidade", TypeName = "varchar(100)")]
    public string? Cidade { get; set; }

    [Required]
    [Column("CargoID")]
    public int CargoId { get; set; }

    [ForeignKey(nameof(CargoId))]
    public Cargo Cargo { get; set; } = null!;
}
