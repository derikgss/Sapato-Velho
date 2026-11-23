using Microsoft.EntityFrameworkCore;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<User> Usuarios => Set<User>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasOne(u => u.Cargo).WithMany(c => c.Usuarios)
            .HasForeignKey(u => u.CargoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.CPF).IsUnique();
        modelBuilder.Entity<Pedido>().HasOne(p => p.Usuario).WithMany()
            .HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ItemPedido>().HasOne(i => i.Pedido).WithMany(p => p.Itens)
            .HasForeignKey(i => i.PedidoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ItemPedido>().HasOne(i => i.Produto).WithMany()
            .HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
    }
}
