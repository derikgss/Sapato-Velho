using System.ComponentModel.DataAnnotations;
namespace SapatoVelho.Api.DTOs;
public sealed class AlterarCargoDto { [Range(1, 3)] public int CargoId { get; set; } }
