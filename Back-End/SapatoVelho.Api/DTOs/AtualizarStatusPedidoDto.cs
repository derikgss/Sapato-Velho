using System.ComponentModel.DataAnnotations;
namespace SapatoVelho.Api.DTOs;
public sealed class AtualizarStatusPedidoDto { [Required, StringLength(30)] public string Status { get; set; } = ""; }
