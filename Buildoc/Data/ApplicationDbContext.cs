using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Buildoc.Models;

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

            // Configurar la relación entre Proyecto y Incidente
            builder.Entity<Proyecto>()
                .HasMany(p => p.Incidentes)
                .WithOne(i => i.Proyecto)
                .HasForeignKey(i => i.ProyectoId);

            // Configurar la relación entre Usuario y Incidente
            builder.Entity<Usuario>()
                .HasMany(u => u.Incidentes)
                .WithOne(i => i.Usuario)
                .HasForeignKey(i => i.UsuarioId);

            // Configurar la relación entre Incidente y Afectados
            builder.Entity<Incidente>()
                .HasMany(i => i.Afectados)
                .WithOne(a => a.Incidente)
                .HasForeignKey(a => a.IncidenteId);

            // Configurar la relación entre Incidente y Seguimientos
            builder.Entity<Incidente>()
                .HasMany(i => i.Seguimientos)
                .WithOne(s => s.Incidente)
                .HasForeignKey(s => s.IncidenteId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            // Configurar la relación entre Usuario y Seguimientos
            builder.Entity<Usuario>()
                .HasMany(u => u.Seguimientos)
                .WithOne(s => s.Usuario)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            // Configurar la relación entre Incidente y TipoIncidente
            builder.Entity<Incidente>()
                .HasOne(i => i.TipoIncidente)
                .WithMany(ti => ti.Incidentes)
                .HasForeignKey(i => i.TipoIncidenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar valor predeterminado para FechaCreacion
            builder.Entity<Incidente>()
                .Property(i => i.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Seguimiento>()
                .Property(s => s.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");
        }

        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Incidente> Incidentes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Afectado> Afectados { get; set; }
        public DbSet<Seguimiento> Seguimientos { get; set; }
        public DbSet<TipoIncidente> TipoIncidentes { get; set; }
        public DbSet<TipoInspeccion> TipoInspeccion { get; set; }
        public DbSet<Buildoc.Models.Inspeccion> Inspeccion { get; set; }
        
    }
}
