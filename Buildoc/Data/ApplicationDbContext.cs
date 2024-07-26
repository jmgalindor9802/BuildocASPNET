using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Buildoc.Models;
using System.Reflection.Emit;

namespace Buildoc.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Usuario>(entityTypeBuilder =>
            {
                entityTypeBuilder.ToTable("Usuarios");

            });

            // Configurar la relación entre Proyecto y Usuario (Coordinador)
            builder.Entity<Proyecto>()
                .HasOne(p => p.Coordinador)
                .WithMany()
                .HasForeignKey(p => p.CoordinadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar la relación muchos a muchos entre Proyecto y Residentes
            builder.Entity<Proyecto>()
                .HasMany(p => p.Residentes)
                .WithMany(u => u.Proyectos)
                .UsingEntity(j => j.ToTable("ProyectoResidentes"));


        }

        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<TipoInspeccion> TipoInspeccion { get; set; }
        public DbSet<Buildoc.Models.Inspeccion> Inspeccion { get; set; }
        
    }
}
