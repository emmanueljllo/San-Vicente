using System;

namespace Hospital_San_Vicente.Models;

public class EmailHistory
{
    public int Id { get; set; }

    // Propiedades de string deben ser inicializadas para evitar el warning CS8618
    // y asegurar que EF Core no intente escribir NULL si no se asignó nada.
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; }
    public bool IsSent { get; set; }
    
    // Clave foránea al objeto Appointment
    public int AppointmentId { get; set; }
    
    // Objeto de navegación (se inicializa con default! para indicarle a C# que EF Core lo inicializará)
    public Appointment Appointment { get; set; } = default!; 
}