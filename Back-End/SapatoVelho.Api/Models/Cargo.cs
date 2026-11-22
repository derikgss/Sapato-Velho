using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SapatoVelho.Api.Models;

[Table("Cargos")]
public class Cargo
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("Nome", TypeName = "varchar(50)")]
    public string Nome { get; set; } = string.Empty;

    // Um cargo pode estar associado a vários usuários.
    public ICollection<User> Usuarios { get; set; } = new List<User>();
}
