using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Buildoc.Models;
using System.Reflection.Emit;
using Buildoc.Models.Inspecciones;

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

              builder.Entity<Inspeccion>()
             .HasOne(i => i.Respuesta)
             .WithOne(r => r.Inspeccion)
             .HasForeignKey<RespuestaInspeccion>(r => r.InspeccionId);

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

            // Configuración de la relación muchos a muchos entre Incidente y Lesionados
            builder.Entity<IncidenteLesionado>()
                .HasOne(il => il.Incidente)
                .WithMany(i => i.IncidenteLesionados)
                .HasForeignKey(il => il.IncidenteId);

            builder.Entity<IncidenteLesionado>()
                .HasOne(il => il.Lesionado)
                .WithMany(l => l.IncidenteLesionados)
                .HasForeignKey(il => il.LesionadoId);

            // Configurar la relación entre Incidente y NovedadesIncidentes
            builder.Entity<Incidente>()
                .HasMany(i => i.NovedadesIncidentes)
                .WithOne(s => s.Incidente)
                .HasForeignKey(s => s.IncidenteId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            // Configurar la relación entre Usuario y NovedadesIncidentes
            builder.Entity<Usuario>()
                .HasMany(u => u.NovedadesIncidentes)
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

            builder.Entity<NovedadesIncidente>()
                .Property(s => s.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");


            builder.Entity<NovedadInspeccion>()
           .HasOne(n => n.Usuario)
           .WithMany() 
           .HasForeignKey(n => n.UsuarioId);

            builder.Entity<TipoInspeccion>()
           .HasMany(t => t.Archivos)
           .WithOne(f => f.TipoInspeccion)
           .HasForeignKey(f => f.TipoInspeccionId)
           .OnDelete(DeleteBehavior.Cascade);



        }

        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<TipoInspeccion> TipoInspeccion { get; set; }
        public DbSet<Buildoc.Models.Inspeccion> Inspeccion { get; set; }
        public DbSet<Incidente> Incidentes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<NovedadesIncidente> NovedadesIncidentes { get; set; }
        public DbSet<TipoIncidente> TipoIncidentes { get; set; }
        public DbSet<Buildoc.Models.RespuestaInspeccion> RespuestaInspeccion { get; set; }
        public DbSet<Inspeccion> Inspecciones { get; set; }

        public DbSet<RespuestaInspeccion> RespuestaInspecciones { get; set; }
        public DbSet<Lesionado> Lesionados { get; set; }
        public DbSet<IncidenteLesionado> IncidenteLesionados { get; set; }
        public DbSet<NovedadInspeccion> NovedadInspeccion { get; set; }

        public DbSet<FileModel> FileModels { get; set; }


    }
}
