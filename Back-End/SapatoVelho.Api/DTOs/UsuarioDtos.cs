using System.ComponentModel.DataAnnotations;

namespace SapatoVelho.Api.DTOs;

public sealed class CadastroUsuarioDto
{
    [Required, StringLength(150, MinimumLength = 2)] public string Nome { get; set; } = "";
    public DateTime? DataNascimento { get; set; }
    [Required, StringLength(20)] public string CPF { get; set; } = "";
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = "";
    [Required, MinLength(8), StringLength(100)] public string Senha { get; set; } = "";
    [StringLength(200)] public string? Endereco { get; set; }
    [StringLength(20)] public string? CEP { get; set; }
    [StringLength(100)] public string? Cidade { get; set; }
}

public sealed class LoginDto
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Senha { get; set; } = "";
}

public sealed class AtualizarUsuarioDto
{
    [Required, StringLength(150, MinimumLength = 2)] public string Nome { get; set; } = "";
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; } = "";
    [StringLength(200)] public string? Endereco { get; set; }
    [StringLength(20)] public string? CEP { get; set; }
    [StringLength(100)] public string? Cidade { get; set; }
    public string? SenhaAtual { get; set; }
    [MinLength(8), StringLength(100)] public string? NovaSenha { get; set; }
}

public sealed record UsuarioResposta(int Id, string Nome, string Email, string CPF, DateTime? DataNascimento, string? Endereco, string? CEP, string? Cidade, int CargoId, string Cargo);
