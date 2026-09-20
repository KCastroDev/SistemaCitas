using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Models;

namespace SistemaCitas.Data;

public class AppDbContext : IdentityDbContext<Usuario>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Provincia> Provincias => Set<Provincia>();
    public DbSet<Distrito> Distritos => Set<Distrito>();
    public DbSet<UnidadEjecutora> UnidadesEjecutoras => Set<UnidadEjecutora>();
    public DbSet<Ipress> Ipress => Set<Ipress>();
    public DbSet<PlanSeguro> PlanesSeguro => Set<PlanSeguro>();
    public DbSet<TipoContrato> TiposContrato => Set<TipoContrato>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Doctor> Doctores => Set<Doctor>();
    public DbSet<DoctorEspecialidad> DoctorEspecialidades => Set<DoctorEspecialidad>();
    public DbSet<HorarioDoctor> HorariosDoctor => Set<HorarioDoctor>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Triaje> Triajes => Set<Triaje>();
    public DbSet<AtencionMedica> AtencionesMedicas => Set<AtencionMedica>();
    public DbSet<Cie10> Cie10 => Set<Cie10>();
    public DbSet<AtencionDiagnostico> AtencionDiagnosticos => Set<AtencionDiagnostico>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<DetalleReceta> DetallesReceta => Set<DetalleReceta>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<HistorialPuntaje> HistorialPuntajes => Set<HistorialPuntaje>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);


        b.Entity<UnidadEjecutora>().HasData(
            new UnidadEjecutora { IdUnidadEjecutora = 1, Codigo = "001-1310", Nombre = "Red de Salud Trujillo" },
            new UnidadEjecutora { IdUnidadEjecutora = 2, Codigo = "001-1311", Nombre = "Hospital Regional Docente de Trujillo" },
            new UnidadEjecutora { IdUnidadEjecutora = 3, Codigo = "001-1312", Nombre = "Red de Salud Sánchez Carrión" });

        b.Entity<Usuario>().ToTable("USUARIO");
        b.Entity<IdentityRole>().ToTable("ROL");
        b.Entity<IdentityUserRole<string>>().ToTable("USUARIO_ROL");
        b.Entity<IdentityUserClaim<string>>().ToTable("USUARIO_CLAIM");
        b.Entity<IdentityUserLogin<string>>().ToTable("USUARIO_LOGIN");
        b.Entity<IdentityUserToken<string>>().ToTable("USUARIO_TOKEN");
        b.Entity<IdentityRoleClaim<string>>().ToTable("ROL_CLAIM");

        // Claves primarias compuestas (relaciones N-N) 
        b.Entity<DoctorEspecialidad>()
            .HasKey(de => new { de.IdDoctor, de.IdEspecialidad });

        b.Entity<AtencionDiagnostico>()
            .HasKey(ad => new { ad.IdAtencion, ad.CodigoCie10 });

        // Indices unicos
        b.Entity<Paciente>().HasIndex(p => p.Dni).IsUnique();
        b.Entity<Doctor>().HasIndex(d => d.Cmp).IsUnique();
        b.Entity<Doctor>().HasIndex(d => d.Dni).IsUnique();
        b.Entity<Ipress>().HasIndex(i => i.CodigoRenipress).IsUnique();
        b.Entity<Distrito>().HasIndex(d => new { d.IdProvincia, d.Nombre }).IsUnique();

        // Un doctor no puede tener dos citas a la misma fecha y hora
        b.Entity<Cita>().HasIndex(c => new { c.IdDoctor, c.FechaCita, c.HoraCita }).IsUnique();

        // ---- Claves foraneas explicitas ----
    
        b.Entity<Provincia>().HasOne(p => p.Departamento).WithMany(d => d.Provincias)
            .HasForeignKey(p => p.IdDepartamento);
        b.Entity<Distrito>().HasOne(d => d.Provincia).WithMany(p => p.Distritos)
            .HasForeignKey(d => d.IdProvincia);

        b.Entity<Ipress>().HasOne(i => i.UnidadEjecutora).WithMany(u => u.Ipress)
            .HasForeignKey(i => i.IdUnidadEjecutora);
        b.Entity<Ipress>().HasOne(i => i.Distrito).WithMany()
            .HasForeignKey(i => i.IdDistrito);

        b.Entity<Usuario>().HasOne(u => u.Ipress).WithMany()
            .HasForeignKey(u => u.IdIpress);

        b.Entity<Paciente>().HasOne(p => p.PlanSeguro).WithMany(ps => ps.Pacientes)
            .HasForeignKey(p => p.IdPlanSeguro);
        b.Entity<Paciente>().HasOne(p => p.Distrito).WithMany()
            .HasForeignKey(p => p.IdDistrito);

        b.Entity<Doctor>().HasOne(d => d.Ipress).WithMany(i => i.Doctores)
            .HasForeignKey(d => d.IdIpress);
        b.Entity<Doctor>().HasOne(d => d.TipoContrato).WithMany(t => t.Doctores)
            .HasForeignKey(d => d.IdTipoContrato);

        b.Entity<DoctorEspecialidad>().HasOne(de => de.Doctor).WithMany(d => d.DoctorEspecialidades)
            .HasForeignKey(de => de.IdDoctor);
        b.Entity<DoctorEspecialidad>().HasOne(de => de.Especialidad).WithMany(e => e.DoctorEspecialidades)
            .HasForeignKey(de => de.IdEspecialidad);

        b.Entity<HorarioDoctor>().HasOne(h => h.Doctor).WithMany(d => d.Horarios)
            .HasForeignKey(h => h.IdDoctor);

        b.Entity<Cita>().HasOne(c => c.Paciente).WithMany(p => p.Citas)
            .HasForeignKey(c => c.IdPaciente);
        b.Entity<Cita>().HasOne(c => c.Doctor).WithMany(d => d.Citas)
            .HasForeignKey(c => c.IdDoctor);
        b.Entity<Cita>().HasOne(c => c.Ipress).WithMany()
            .HasForeignKey(c => c.IdIpress);

        b.Entity<AtencionDiagnostico>().HasOne(ad => ad.AtencionMedica).WithMany(a => a.Diagnosticos)
            .HasForeignKey(ad => ad.IdAtencion);
        b.Entity<AtencionDiagnostico>().HasOne(ad => ad.Cie10).WithMany()
            .HasForeignKey(ad => ad.CodigoCie10);

        b.Entity<DetalleReceta>().HasOne(dr => dr.AtencionMedica).WithMany(a => a.Receta)
            .HasForeignKey(dr => dr.IdAtencion);
        b.Entity<DetalleReceta>().HasOne(dr => dr.Medicamento).WithMany()
            .HasForeignKey(dr => dr.IdMedicamento);

        b.Entity<HistorialPuntaje>().HasOne(h => h.Paciente).WithMany(p => p.HistorialPuntajes)
            .HasForeignKey(h => h.IdPaciente);
        b.Entity<HistorialPuntaje>().HasOne(h => h.Cita).WithMany()
            .HasForeignKey(h => h.IdCita);

        b.Entity<Auditoria>().HasOne(a => a.Usuario).WithMany()
            .HasForeignKey(a => a.UsuarioId);

        // ---- Relaciones 1 a 1 ----
        b.Entity<Triaje>()
            .HasOne(t => t.Cita).WithOne(c => c.Triaje)
            .HasForeignKey<Triaje>(t => t.IdCita)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<AtencionMedica>()
            .HasOne(a => a.Cita).WithOne(c => c.AtencionMedica)
            .HasForeignKey<AtencionMedica>(a => a.IdCita)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- Cuenta opcional de paciente y doctor ----
        b.Entity<Paciente>()
            .HasOne(p => p.Usuario).WithOne()
            .HasForeignKey<Paciente>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Doctor>()
            .HasOne(d => d.Usuario).WithOne()
            .HasForeignKey<Doctor>(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Paciente>()
            .HasOne(p => p.RegistradoPor).WithMany()
            .HasForeignKey(p => p.RegistradoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        
        foreach (var fk in b.Model.GetEntityTypes()
                                  .SelectMany(t => t.GetForeignKeys())
                                  .Where(fk => !fk.IsOwnership
                                            && fk.DeleteBehavior == DeleteBehavior.Cascade
                                            && fk.DeclaringEntityType.ClrType != typeof(Triaje)
                                            && fk.DeclaringEntityType.ClrType != typeof(AtencionMedica)
                                            && fk.DeclaringEntityType.ClrType != typeof(AtencionDiagnostico)
                                            && fk.DeclaringEntityType.ClrType != typeof(DetalleReceta)))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        b.Entity<AtencionDiagnostico>().HasOne(ad => ad.Cie10).WithMany()
            .HasForeignKey(ad => ad.CodigoCie10).OnDelete(DeleteBehavior.Restrict);
        b.Entity<DetalleReceta>().HasOne(dr => dr.Medicamento).WithMany()
            .HasForeignKey(dr => dr.IdMedicamento).OnDelete(DeleteBehavior.Restrict);

        // ---- Enum guardado como texto legible ----
        b.Entity<Cita>()
            .Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(15);

        // ---- Datos semilla minimos ----
        b.Entity<Departamento>().HasData(
            new Departamento { IdDepartamento = "13", Nombre = "La Libertad" });

        b.Entity<Provincia>().HasData(
            new Provincia { IdProvincia = "1301", Nombre = "Trujillo", IdDepartamento = "13" },
            new Provincia { IdProvincia = "1302", Nombre = "Ascope", IdDepartamento = "13" },
            new Provincia { IdProvincia = "1311", Nombre = "Santiago de Chuco", IdDepartamento = "13" });

        b.Entity<Distrito>().HasData(
            new Distrito { IdDistrito = "130101", Nombre = "Trujillo", IdProvincia = "1301" },
            new Distrito { IdDistrito = "130104", Nombre = "Laredo", IdProvincia = "1301" },
            new Distrito { IdDistrito = "130105", Nombre = "Moche", IdProvincia = "1301" },
            new Distrito { IdDistrito = "130109", Nombre = "Huanchaco", IdProvincia = "1301" });

        b.Entity<PlanSeguro>().HasData(
            new PlanSeguro { IdPlanSeguro = 1, Nombre = "SIS Gratuito" },
            new PlanSeguro { IdPlanSeguro = 2, Nombre = "SIS Para Todos" },
            new PlanSeguro { IdPlanSeguro = 3, Nombre = "SIS Independiente" });

        b.Entity<TipoContrato>().HasData(
            new TipoContrato { IdTipoContrato = 1, Nombre = "Honorarios", HorasSemanales = 12 },
            new TipoContrato { IdTipoContrato = 2, Nombre = "Part time", HorasSemanales = 24 },
            new TipoContrato { IdTipoContrato = 3, Nombre = "Full day", HorasSemanales = 48 });

        b.Entity<Especialidad>().HasData(
            new Especialidad { IdEspecialidad = 1, Nombre = "Medicina General" },
            new Especialidad { IdEspecialidad = 2, Nombre = "Pediatria" },
            new Especialidad { IdEspecialidad = 3, Nombre = "Ginecologia" },
            new Especialidad { IdEspecialidad = 4, Nombre = "Cirugia" });

        b.Entity<Cie10>().HasData(
            new Cie10 { CodigoCie10 = "J00", Descripcion = "Rinofaringitis aguda (resfriado comun)" },
            new Cie10 { CodigoCie10 = "E11", Descripcion = "Diabetes mellitus tipo 2" },
            new Cie10 { CodigoCie10 = "I10", Descripcion = "Hipertension esencial (primaria)" });
    }
}