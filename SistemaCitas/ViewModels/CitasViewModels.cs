using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaCitas.Models;
using SistemaCitas.Services;

namespace SistemaCitas.ViewModels;

// Datos de la pantalla "buscar cupos y reservar" (RF-07). La usan el paciente (web) y Admision (presencial).
public class BuscarCuposViewModel
{
    public Paciente Paciente { get; set; } = null!;

    // Lo que eligio el usuario
    public int? IdEspecialidad { get; set; }
    public int? IdDoctor { get; set; }
    public DateOnly? Fecha { get; set; }

    // Menus desplegables y resultados
    public List<SelectListItem> Especialidades { get; set; } = new();
    public List<SelectListItem> Doctores { get; set; } = new();
    public Doctor? DoctorSeleccionado { get; set; }
    public List<CupoDto> Cupos { get; set; } = new();

    // Prioridad (RF-08)
    public string? AvisoPrioridad { get; set; }
    public bool Bloqueado { get; set; }

    // true = asignacion presencial hecha por Admision (RF-09, exige verificar el DNI fisico)
    public bool Presencial { get; set; }
}

// Pantalla "Mis citas" del paciente
public class MisCitasViewModel
{
    public Paciente Paciente { get; set; } = null!;
    public IEnumerable<Cita> Citas { get; set; } = new List<Cita>();
    public string Aviso { get; set; } = "";
    public bool Bloqueado { get; set; }
}

// Pantalla "Citas del dia" de Admision (RF-10 / RF-11)
public class CitasDelDiaViewModel
{
    public DateOnly Fecha { get; set; }
    public IEnumerable<Cita> Citas { get; set; } = new List<Cita>();
}
