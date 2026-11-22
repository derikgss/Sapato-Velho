using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<User> Usuarios => Set<User>();
    public DbSet<Cargo> Cargos => Set<Cargo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Um cargo pode estar associado a vários usuários.
        // Cada usuário possui um único cargo.
        modelBuilder.Entity<User>()
            .HasOne(usuario => usuario.Cargo)
            .WithMany(cargo => cargo.Usuarios)
            .HasForeignKey(usuario => usuario.CargoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Evita cadastrar duas contas com o mesmo e-mail ou CPF.
        modelBuilder.Entity<User>()
            .HasIndex(usuario => usuario.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(usuario => usuario.CPF)
            .IsUnique();
    }
}
