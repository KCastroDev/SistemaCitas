using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Data;
using SistemaCitas.Models;
using SistemaCitas.Repositories;
using SistemaCitas.ViewModels;

namespace SistemaCitas.Services;

public class DoctorService : IDoctorService
{
    private const string Habilitado = "Habilitado";
    private const string NoHabilitado = "No habilitado";

    private readonly IDoctorRepositorio _doctores;
    private readonly IRepositorio<HorarioDoctor> _horarios;
    private readonly IRepositorio<Ipress> _ipress;
    private readonly IRepositorio<TipoContrato> _contratos;
    private readonly IRepositorio<Especialidad> _especialidades;
    private readonly UserManager<Usuario> _userManager;
    private readonly IAuditoriaService _auditoria;

    public DoctorService(
        IDoctorRepositorio doctores,
        IRepositorio<HorarioDoctor> horarios,
        IRepositorio<Ipress> ipress,
        IRepositorio<TipoContrato> contratos,
        IRepositorio<Especialidad> especialidades,
        UserManager<Usuario> userManager,
        IAuditoriaService auditoria)
    {
        _doctores = doctores;
        _horarios = horarios;
        _ipress = ipress;
        _contratos = contratos;
        _especialidades = especialidades;
        _userManager = userManager;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Doctor>> ListarAsync(string? buscar) => _doctores.ListarAsync(buscar);

    public Task<Doctor?> ObtenerAsync(int id) => _doctores.ObtenerConDetalleAsync(id);

    public async Task CargarListasAsync(DoctorViewModel vm)
    {
        var ipress = await _ipress.BuscarAsync(i => i.Activo);
        vm.Ipresses = ipress.OrderBy(i => i.Nombre)
            .Select(i => new SelectListItem($"{i.Nombre} ({i.CodigoRenipress})", i.IdIpress.ToString()))
            .ToList();

        var contratos = await _contratos.ObtenerTodosAsync();
        vm.Contratos = contratos.OrderBy(c => c.HorasSemanales)
            .Select(c => new SelectListItem($"{c.Nombre} ({c.HorasSemanales} h/semana)", c.IdTipoContrato.ToString()))
            .ToList();

        var especialidades = await _especialidades.BuscarAsync(e => e.Activo);
        vm.Especialidades = especialidades.OrderBy(e => e.Nombre)
            .Select(e => new SelectListItem(e.Nombre, e.IdEspecialidad.ToString()))
            .ToList();
    }

    // RF-04: registra al doctor y crea su cuenta de acceso con el rol "Doctor".
    public async Task<ResultadoOperacion> RegistrarAsync(DoctorViewModel vm)
    {
        if (await _doctores.ExisteAsync(d => d.Dni == vm.Dni))
            return ResultadoOperacion.Falla("Ya existe un doctor con ese DNI.");

        if (await _doctores.ExisteAsync(d => d.Cmp == vm.Cmp))
            return ResultadoOperacion.Falla("Ya existe un doctor con ese CMP.");

        if (await _userManager.FindByEmailAsync(vm.Correo) != null)
            return ResultadoOperacion.Falla("Ya existe una cuenta con ese correo.");

        if (!await _ipress.ExisteAsync(i => i.IdIpress == vm.IdIpress && i.Activo))
            return ResultadoOperacion.Falla("La IPRESS seleccionada no existe o está inhabilitada.");

        if (!await _contratos.ExisteAsync(c => c.IdTipoContrato == vm.IdTipoContrato))
            return ResultadoOperacion.Falla("El tipo de contrato seleccionado no existe.");

        if (!await _especialidades.ExisteAsync(e => e.IdEspecialidad == vm.IdEspecialidad && e.Activo))
            return ResultadoOperacion.Falla("La especialidad seleccionada no existe o está inactiva.");

        // 1) Cuenta de acceso
        var usuario = new Usuario
        {
            UserName = vm.Correo,
            Email = vm.Correo,
            EmailConfirmed = true,
            Nombres = $"{vm.Nombres} {vm.ApellidoPaterno}",
            IdIpress = vm.IdIpress,
            Activo = true
        };

        var creado = await _userManager.CreateAsync(usuario, vm.Contrasena);
        if (!creado.Succeeded)
            return ResultadoOperacion.Falla(TraducirError(creado.Errors.First()));

        await _userManager.AddToRoleAsync(usuario, Roles.Doctor);

        // 2) Doctor + su especialidad. Si algo falla, se elimina la cuenta para no dejarla huerfana.
        try
        {
            var doctor = new Doctor
            {
                Cmp = vm.Cmp,
                Dni = vm.Dni,
                Nombres = vm.Nombres.Trim(),
                ApellidoPaterno = vm.ApellidoPaterno.Trim(),
                ApellidoMaterno = vm.ApellidoMaterno.Trim(),
                EstadoHabilitacion = "No verificado",
                IdIpress = vm.IdIpress!.Value,
                IdTipoContrato = vm.IdTipoContrato!.Value,
                UsuarioId = usuario.Id,
                Activo = true
            };
            doctor.DoctorEspecialidades.Add(new DoctorEspecialidad { IdEspecialidad = vm.IdEspecialidad!.Value });

            await _doctores.AgregarAsync(doctor);
            await _doctores.GuardarCambiosAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(usuario);
            return ResultadoOperacion.Falla("No se pudo guardar el doctor. Intente nuevamente.");
        }

        await _auditoria.RegistrarAsync("Doctor", "Crear", vm.Cmp, null,
            $"CMP {vm.Cmp} - {vm.Nombres} {vm.ApellidoPaterno} {vm.ApellidoMaterno}");

        return ResultadoOperacion.Ok();
    }

    // RF-05 / CU-05: validacion del CMP. En esta fase es SIMULADA (restriccion RO-03 del informe):
    // no hay conexion real al Colegio Medico del Peru. Regla de la simulacion:
    // un CMP que termina en 0 figura como "No habilitado"; cualquier otro, como "Habilitado".
    public async Task<ResultadoOperacion> ValidarCmpAsync(int idDoctor)
    {
        var doctor = await _doctores.ObtenerPorIdAsync(idDoctor);
        if (doctor == null) return ResultadoOperacion.Falla("El doctor no existe.");

        var estadoAnterior = doctor.EstadoHabilitacion;
        var habilitado = CmpIntegrationService.EstaHabilitado(doctor.Cmp);
        doctor.EstadoHabilitacion = habilitado ? Habilitado : NoHabilitado;
        doctor.FechaValidacionCmp = DateTime.Now;
        await _doctores.GuardarCambiosAsync();

        await _auditoria.RegistrarAsync("Doctor", "Editar", doctor.Cmp,
            $"Habilitación: {estadoAnterior}", $"Habilitación: {doctor.EstadoHabilitacion}");

        return habilitado
            ? ResultadoOperacion.Ok()
            : ResultadoOperacion.Falla($"El CMP {doctor.Cmp} figura como NO habilitado (validación simulada).");
    }

    // Baja logica: el doctor y su cuenta se desactivan, pero no se borra nada.
    public async Task<ResultadoOperacion> CambiarEstadoAsync(int idDoctor)
    {
        var doctor = await _doctores.ObtenerPorIdAsync(idDoctor);
        if (doctor == null) return ResultadoOperacion.Falla("El doctor no existe.");

        var activoAnterior = doctor.Activo;
        doctor.Activo = !doctor.Activo;
        await _doctores.GuardarCambiosAsync();

        await _auditoria.RegistrarAsync("Doctor", "Editar", doctor.Cmp,
            activoAnterior ? "Activo" : "Inactivo", doctor.Activo ? "Activo" : "Inactivo");

        if (doctor.UsuarioId != null)
        {
            var usuario = await _userManager.FindByIdAsync(doctor.UsuarioId);
            if (usuario != null)
            {
                usuario.Activo = doctor.Activo;
                await _userManager.UpdateAsync(usuario);
            }
        }

        return ResultadoOperacion.Ok();
    }

    // RF-06 / RC-02: horarios estrictos segun el tipo de contrato.
    public async Task<ResultadoOperacion> AgregarHorarioAsync(int idDoctor, HorarioViewModel vm)
    {
        var doctor = await _doctores.ObtenerConDetalleAsync(idDoctor);
        if (doctor == null) return ResultadoOperacion.Falla("El doctor no existe.");
        if (!doctor.Activo) return ResultadoOperacion.Falla("El doctor está inactivo; no se le pueden asignar horarios.");

        if (doctor.EstadoHabilitacion != Habilitado)
            return ResultadoOperacion.Falla("Primero valide la habilitación del CMP del doctor (debe figurar como Habilitado).");

        var inicio = vm.HoraInicio!.Value;
        var fin = vm.HoraFin!.Value;

        if (fin <= inicio)
            return ResultadoOperacion.Falla("La hora de fin debe ser mayor que la hora de inicio.");

        if (inicio < new TimeOnly(6, 0) || fin > new TimeOnly(22, 0))
            return ResultadoOperacion.Falla("El horario de atención va de 06:00 a 22:00.");

        var horas = (fin - inicio).TotalHours;
        if (horas < 1)
            return ResultadoOperacion.Falla("Cada bloque debe durar al menos 1 hora.");

        var activos = doctor.Horarios.Where(h => h.Activo).ToList();

        // No puede haber dos bloques que se crucen el mismo dia
        if (activos.Any(h => h.DiaSemana == vm.DiaSemana && inicio < h.HoraFin && fin > h.HoraInicio))
            return ResultadoOperacion.Falla("Ese bloque se cruza con otro horario del mismo día.");

        // RC-02: no se puede programar mas horas de las que permite su contrato
        var asignadas = activos.Sum(h => (h.HoraFin - h.HoraInicio).TotalHours);
        var limite = doctor.TipoContrato.HorasSemanales;
        if (asignadas + horas > limite)
            return ResultadoOperacion.Falla(
                $"RC-02: el contrato {doctor.TipoContrato.Nombre} permite {limite} horas semanales. " +
                $"Ya tiene {asignadas:0.##} h asignadas y este bloque suma {horas:0.##} h.");

        await _horarios.AgregarAsync(new HorarioDoctor
        {
            IdDoctor = idDoctor,
            DiaSemana = vm.DiaSemana,
            HoraInicio = inicio,
            HoraFin = fin,
            CuposPorHora = vm.CuposPorHora,
            Activo = true
        });
        await _horarios.GuardarCambiosAsync();

        await _auditoria.RegistrarAsync("HorarioDoctor", "Crear", doctor.Cmp, null,
            $"Día {vm.DiaSemana}, {inicio:HH:mm} a {fin:HH:mm}, {vm.CuposPorHora} cupos por hora");

        return ResultadoOperacion.Ok();
    }

    public async Task<ResultadoOperacion> QuitarHorarioAsync(int idHorario)
    {
        var horario = await _horarios.ObtenerPorIdAsync(idHorario);
        if (horario == null) return ResultadoOperacion.Falla("El horario no existe.");

        _horarios.Eliminar(horario);
        await _horarios.GuardarCambiosAsync();

        await _auditoria.RegistrarAsync("HorarioDoctor", "Eliminar", horario.IdHorario.ToString(),
            $"Doctor {horario.IdDoctor}, día {horario.DiaSemana}, {horario.HoraInicio:HH:mm} a {horario.HoraFin:HH:mm}", null);
        return ResultadoOperacion.Ok();
    }

    // Traduce los mensajes de Identity (vienen en ingles) a espanol.
    private static string TraducirError(IdentityError e) => e.Code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo.",
        "PasswordTooShort" => "La contraseña es demasiado corta.",
        "PasswordRequiresDigit" => "La contraseña debe tener al menos un número.",
        "PasswordRequiresLower" => "La contraseña debe tener al menos una minúscula.",
        "PasswordRequiresUpper" => "La contraseña debe tener al menos una mayúscula.",
        "PasswordRequiresNonAlphanumeric" => "La contraseña debe tener al menos un símbolo.",
        _ => e.Description
    };
}
