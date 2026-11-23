using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SapatoVelho.Api.Data;
using SapatoVelho.Api.DTOs;
using SapatoVelho.Api.Models;
using SapatoVelho.Api.Services;

namespace SapatoVelho.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController(AppDbContext db, IConfiguration config, IPasswordService passwords) : ControllerBase
{
    private static UsuarioResposta ParaResposta(User u) => new(u.Id, u.Nome, u.Email, u.CPF, u.DataNascimento, u.Endereco, u.CEP, u.Cidade, u.CargoId, u.Cargo?.Nome ?? "");
    private string GerarToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(config.GetValue("Jwt:ExpiraEmMinutos", 120));
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Nome),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Cargo.Nome)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims, expires: expires, signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastro(CadastroUsuarioDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var cpf = dto.CPF.Trim();
        if (await db.Usuarios.AnyAsync(u => u.Email == email || u.CPF == cpf))
            return Conflict(new { mensagem = "Já existe uma conta com este e-mail ou CPF." });
        var cargoCliente = await db.Cargos.SingleOrDefaultAsync(c => c.Nome == "Cliente");
        if (cargoCliente is null) return Problem("Os cargos iniciais não foram cadastrados no banco.");
        var user = new User { Nome = dto.Nome.Trim(), DataNascimento = dto.DataNascimento, CPF = cpf, Email = email,
            Endereco = dto.Endereco?.Trim(), CEP = dto.CEP?.Trim(), Cidade = dto.Cidade?.Trim(), CargoId = cargoCliente.Id };
        user.Senha = passwords.Hash(user, dto.Senha);
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();
        user.Cargo = cargoCliente;
        return Created("/api/usuarios/me", ParaResposta(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await db.Usuarios.Include(u => u.Cargo).SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || !passwords.Verify(user, user.Senha, dto.Senha, out var rehash))
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        if (rehash) { user.Senha = passwords.Hash(user, dto.Senha); await db.SaveChangesAsync(); }
        var expires = DateTime.UtcNow.AddMinutes(config.GetValue("Jwt:ExpiraEmMinutos", 120));
        return Ok(new { token = GerarToken(user), expiraEm = expires, usuario = ParaResposta(user) });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await db.Usuarios.Include(u => u.Cargo).AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
        return user is null ? NotFound(new { mensagem = "Usuário não encontrado." }) : Ok(ParaResposta(user));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> Atualizar(AtualizarUsuarioDto dto)
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await db.Usuarios.SingleOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound(new { mensagem = "Usuário não encontrado." });
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Id != id && u.Email == email))
            return Conflict(new { mensagem = "Este e-mail já está sendo utilizado." });
        if (!string.IsNullOrWhiteSpace(dto.NovaSenha))
        {
            if (string.IsNullOrWhiteSpace(dto.SenhaAtual) || !passwords.Verify(user, user.Senha, dto.SenhaAtual, out _))
                return BadRequest(new { mensagem = "Informe a senha atual corretamente para alterá-la." });
            user.Senha = passwords.Hash(user, dto.NovaSenha);
        }
        user.Nome = dto.Nome.Trim(); user.Email = email;
        if (dto.Endereco is not null) user.Endereco = dto.Endereco.Trim();
        if (dto.CEP is not null) user.CEP = dto.CEP.Trim();
        if (dto.Cidade is not null) user.Cidade = dto.Cidade.Trim();
        await db.SaveChangesAsync();
        await db.Entry(user).Reference(u => u.Cargo).LoadAsync();
        return Ok(new { token = GerarToken(user), usuario = ParaResposta(user), mensagem = "Dados atualizados." });
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> ExcluirConta()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await db.Usuarios.SingleOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();
        var pedidos = await db.Pedidos.Where(p => p.UsuarioId == id).Select(p => p.Id).ToListAsync();
        var itens = db.ItensPedido.Where(i => pedidos.Contains(i.PedidoId));
        db.ItensPedido.RemoveRange(itens);
        db.Pedidos.RemoveRange(db.Pedidos.Where(p => p.UsuarioId == id));
        db.Usuarios.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Usuarios.Include(u => u.Cargo).AsNoTracking().Select(u => new UsuarioResposta(u.Id, u.Nome, u.Email, u.CPF, u.DataNascimento, u.Endereco, u.CEP, u.Cidade, u.CargoId, u.Cargo.Nome)).ToListAsync());


    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/cargo")]
    public async Task<IActionResult> AlterarCargo(int id, AlterarCargoDto dto)
    {
        var user = await db.Usuarios.SingleOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound(new { mensagem = "Usuário não encontrado." });
        var cargo = await db.Cargos.SingleOrDefaultAsync(c => c.Id == dto.CargoId);
        if (cargo is null) return BadRequest(new { mensagem = "Cargo inválido." });
        user.CargoId = cargo.Id;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Funcionario")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var u = await db.Usuarios.Include(x => x.Cargo).AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return u is null ? NotFound() : Ok(ParaResposta(u));
    }
}
