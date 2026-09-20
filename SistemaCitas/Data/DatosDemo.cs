using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaCitas.Models;

namespace SistemaCitas.Data;

// Datos de DEMOSTRACION para no tener que registrar todo a mano cada vez que se prueba el sistema.
// Solo se ejecuta cuando la app corre en modo Development (ver Program.cs).
// Es seguro repetirlo: cada registro se crea solo si todavia no existe (se revisa por DNI / CMP / codigo).
//
// Cuentas que crea (todas con contrasena de demo):
//   Doctores  : dr.ramirez@sistemacitas.pe ... (contrasena: Doctor2026)
//   Pacientes : maria.quispe@correo.com, pedro.alarcon@correo.com, carmen.rojas@correo.com (contrasena: Paciente2026)
// Los pacientes tienen distinta importancia para probar las reglas de prioridad (RF-08):
//   Maria 100% (sin restricciones), Pedro 70% (solo cupos con 3+ dias de anticipacion), Carmen 40% (bloqueada).
public static class DatosDemo
{
    private const string ContrasenaDoctor = "Doctor2026";
    private const string ContrasenaPaciente = "Paciente2026";

    // Horario de un doctor: dias (1 = lunes ... 7 = domingo), hora de inicio y de fin
    private record Bloque(int[] Dias, int Desde, int Hasta, int CuposPorHora = 4);

    private record DoctorDemo(string Dni, string Cmp, string Nombres, string Paterno, string Materno,
                              string Correo, int IdxIpress, int IdContrato, string Especialidad, Bloque[] Horario);

    private record PacienteDemo(string Dni, string Nombres, string Paterno, string Materno, DateOnly Nacimiento,
                                string Sexo, string Telefono, int IdPlan, string IdDistrito, decimal Importancia, string? Correo);

    // Contratos (semilla): 1 = Honorarios 12 h, 2 = Part time 24 h, 3 = Full day 48 h
    private static readonly DoctorDemo[] Doctores =
    {
        new("41234501", "70121", "Carlos",   "Ramírez",  "Vega",    "dr.ramirez@sistemacitas.pe",  0, 3, "Medicina General",
            new[] { new Bloque(new[] {1,2,3,4,5,6,7}, 8, 12) }),                                 // 28 h (incluye domingo)
        new("41234502", "70122", "Lucía",    "Torres",   "Paredes", "dra.torres@sistemacitas.pe",  0, 2, "Pediatria",
            new[] { new Bloque(new[] {1,2,3,4,5,7}, 8, 12) }),                                   // 24 h (incluye domingo)
        new("41234503", "70123", "Miguel",   "Castillo", "Rojas",   "dr.castillo@sistemacitas.pe", 2, 2, "Ginecologia",
            new[] { new Bloque(new[] {2,4,6,7}, 14, 18) }),                                      // 16 h (incluye domingo)
        new("41234504", "70124", "Rosa",     "Salazar",  "Quispe",  "dra.salazar@sistemacitas.pe", 2, 1, "Medicina General",
            new[] { new Bloque(new[] {1,3,6}, 8, 12) }),                                         // 12 h
        new("41234505", "70125", "Jorge",    "Mendoza",  "Flores",  "dr.mendoza@sistemacitas.pe",  1, 3, "Cirugia",
            new[] { new Bloque(new[] {1,2,3,4,5,7}, 8, 14), new Bloque(new[] {6}, 8, 12) }),     // 40 h (incluye domingo)
        new("41234506", "70126", "Ana",      "Paredes",  "León",    "dra.paredes@sistemacitas.pe", 1, 2, "Pediatria",
            new[] { new Bloque(new[] {1,2,3,4,5}, 14, 18), new Bloque(new[] {6}, 8, 12) }),      // 24 h
        new("41234507", "70127", "Luis",     "Vargas",   "Cruz",    "dr.vargas@sistemacitas.pe",   1, 3, "Ginecologia",
            new[] { new Bloque(new[] {1,2,3,4,5,6,7}, 9, 13, 2) }),                              // 28 h (incluye domingo)
        new("41234508", "70128", "Patricia", "Díaz",     "Luna",    "dra.diaz@sistemacitas.pe",    0, 2, "Medicina General",
            new[] { new Bloque(new[] {1,2,3,4,5,7}, 14, 18) }),                                  // 24 h (incluye domingo)
        // CMP que termina en 0: la validacion simulada lo marca "No habilitado" (RF-05). No se le crean horarios.
        new("41234509", "70120", "Roberto",  "Chávez",   "Ibáñez",  "dr.chavez@sistemacitas.pe",   2, 1, "Cirugia",
            Array.Empty<Bloque>()),
    };

    private static readonly PacienteDemo[] Pacientes =
    {
        new("71000001", "María Elena", "Quispe",   "Huamán",  new(1990, 3, 14), "F", "987000001", 1, "130101", 100m, "maria.quispe@correo.com"),
        new("71000002", "Pedro",       "Alarcón",  "Silva",   new(1985, 7, 2),  "M", "987000002", 2, "130104", 70m,  "pedro.alarcon@correo.com"),
        new("71000003", "Carmen",      "Rojas",    "Díaz",    new(1978, 11, 30),"F", "987000003", 1, "130105", 40m,  "carmen.rojas@correo.com"),
        new("71000004", "Luis",        "Gómez",    "Carrasco",new(2001, 1, 9),  "M", "987000004", 1, "130101", 100m, null),
        new("71000005", "Rosa",        "Villanueva","Cruz",   new(1969, 5, 21), "F", "987000005", 3, "130109", 100m, null),
        new("71000006", "José",        "Paredes",  "Mori",    new(1995, 9, 17), "M", "987000006", 2, "130101", 90m,  null),
        new("71000007", "Andrea",      "León",     "Ramos",   new(2015, 12, 5), "F", "987000007", 1, "130104", 100m, null),
        new("71000008", "Manuel",      "Sánchez",  "Ortiz",   new(1958, 2, 27), "M", "987000008", 1, "130105", 60m,  null),
        new("71000009", "Elena",       "Torres",   "Bazán",   new(1992, 8, 11), "F", "987000009", 3, "130109", 100m, null),
        new("71000010", "Diego",       "Cabrera",  "Lozano",  new(2008, 6, 3),  "M", "987000010", 2, "130101", 100m, null),
    };

    // Convierte el horario de demo en filas de HORARIO_DOCTOR
    private static List<HorarioDoctor> ArmarHorarios(DoctorDemo d)
    {
        var lista = new List<HorarioDoctor>();
        foreach (var bloque in d.Horario)
            foreach (var dia in bloque.Dias)
                lista.Add(new HorarioDoctor
                {
                    DiaSemana = dia,
                    HoraInicio = new TimeOnly(bloque.Desde, 0),
                    HoraFin = new TimeOnly(bloque.Hasta, 0),
                    CuposPorHora = bloque.CuposPorHora,
                    Activo = true
                });
        return lista;
    }

    // Si el horario guardado del doctor de demo es distinto al definido aqui, lo reemplaza
    private static async Task SincronizarHorariosAsync(AppDbContext db, Doctor doctor, DoctorDemo d)
    {
        if (doctor.EstadoHabilitacion != "Habilitado") return;

        var deseado = ArmarHorarios(d);
        var actual = doctor.Horarios.Where(h => h.Activo).ToList();

        static string Clave(HorarioDoctor h) => $"{h.DiaSemana}|{h.HoraInicio}|{h.HoraFin}|{h.CuposPorHora}";
        var iguales = actual.Count == deseado.Count &&
                      actual.Select(Clave).OrderBy(x => x).SequenceEqual(deseado.Select(Clave).OrderBy(x => x));
        if (iguales) return;

        db.RemoveRange(doctor.Horarios);
        foreach (var h in deseado)
        {
            h.IdDoctor = doctor.IdDoctor;
            db.Add(h);
        }
        await db.SaveChangesAsync();
    }

    public static async Task CargarAsync(IServiceProvider servicios)
    {
        var db = servicios.GetRequiredService<AppDbContext>();
        var userManager = servicios.GetRequiredService<UserManager<Usuario>>();

        var admin = await userManager.FindByEmailAsync("admin@sistemacitas.pe");

        // 1) IPRESS de demo
        var ipressDemo = new[]
        {
            ("00004501", "C.S. Alto Trujillo",                    "I",    "Av. Alto Trujillo 123", 1, "130101"),
            ("00004502", "Hospital Regional Docente de Trujillo", "II",  "Av. Mansiche 795",      2, "130101"),
            ("00004503", "C.S. Huanchaco",                        "I",    "Calle Los Pinos 200",   1, "130109"),
        };
        var listaIpress = new List<Ipress>();
        foreach (var (codigo, nombre, nivel, direccion, ue, distrito) in ipressDemo)
        {
            var ipress = await db.Set<Ipress>().FirstOrDefaultAsync(i => i.CodigoRenipress == codigo);
            if (ipress == null)
            {
                ipress = new Ipress
                {
                    CodigoRenipress = codigo, Nombre = nombre, NivelAtencion = nivel, Direccion = direccion,
                    IdUnidadEjecutora = ue, IdDistrito = distrito, Activo = true
                };
                db.Add(ipress);
                await db.SaveChangesAsync();
            }
            listaIpress.Add(ipress);
        }

        // 2) Doctores con su cuenta, especialidad y horarios
        foreach (var d in Doctores)
        {
            // Si el doctor de demo ya existe, solo se actualizan sus horarios (por ejemplo, para agregar domingos)
            var existente = await db.Set<Doctor>().Include(x => x.Horarios).FirstOrDefaultAsync(x => x.Dni == d.Dni);
            if (existente != null)
            {
                await SincronizarHorariosAsync(db, existente, d);
                continue;
            }
            if (await db.Set<Doctor>().AnyAsync(x => x.Cmp == d.Cmp)) continue;
            if (await userManager.FindByEmailAsync(d.Correo) != null) continue;

            var especialidad = await db.Set<Especialidad>().FirstOrDefaultAsync(e => e.Nombre == d.Especialidad);
            if (especialidad == null) continue;   // si borraron la especialidad, se omite este doctor

            var ipress = listaIpress[d.IdxIpress];

            var usuario = new Usuario
            {
                UserName = d.Correo, Email = d.Correo, EmailConfirmed = true,
                Nombres = $"{d.Nombres} {d.Paterno}", IdIpress = ipress.IdIpress, Activo = true
            };
            var creado = await userManager.CreateAsync(usuario, ContrasenaDoctor);
            if (!creado.Succeeded) continue;
            await userManager.AddToRoleAsync(usuario, Roles.Doctor);

            var habilitado = !d.Cmp.EndsWith('0');   // misma regla simulada del DoctorService (RO-03)
            var doctor = new Doctor
            {
                Cmp = d.Cmp, Dni = d.Dni, Nombres = d.Nombres, ApellidoPaterno = d.Paterno, ApellidoMaterno = d.Materno,
                EstadoHabilitacion = habilitado ? "Habilitado" : "No habilitado",
                FechaValidacionCmp = DateTime.Now,
                IdIpress = ipress.IdIpress, IdTipoContrato = d.IdContrato, UsuarioId = usuario.Id, Activo = true
            };
            doctor.DoctorEspecialidades.Add(new DoctorEspecialidad { IdEspecialidad = especialidad.IdEspecialidad });

            if (habilitado)
            {
                foreach (var h in ArmarHorarios(d))
                    doctor.Horarios.Add(h);
            }

            db.Add(doctor);
            await db.SaveChangesAsync();
        }

        // 3) Pacientes (algunos con cuenta web) con distinta importancia
        foreach (var p in Pacientes)
        {
            if (await db.Set<Paciente>().AnyAsync(x => x.Dni == p.Dni)) continue;

            string? usuarioId = null;
            if (p.Correo != null && await userManager.FindByEmailAsync(p.Correo) == null)
            {
                var usuario = new Usuario
                {
                    UserName = p.Correo, Email = p.Correo, EmailConfirmed = true,
                    Nombres = $"{p.Nombres} {p.Paterno}", Activo = true
                };
                var creado = await userManager.CreateAsync(usuario, ContrasenaPaciente);
                if (creado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, Roles.Paciente);
                    usuarioId = usuario.Id;
                }
            }

            db.Add(new Paciente
            {
                Dni = p.Dni, Nombres = p.Nombres, ApellidoPaterno = p.Paterno, ApellidoMaterno = p.Materno,
                FechaNacimiento = p.Nacimiento, Sexo = p.Sexo, Telefono = p.Telefono,
                IdPlanSeguro = p.IdPlan, IdDistrito = p.IdDistrito, PorcentajeImportancia = p.Importancia,
                UsuarioId = usuarioId, RegistradoPorId = admin?.Id, Activo = true
            });
            await db.SaveChangesAsync();
        }
    }
}
