using Hospital_San_Vicente.Data;
using Hospital_San_Vicente.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration; // Opcional: si tu EmailService necesita configuración SMTP

namespace Hospital_San_Vicente.Services;

public class EmailService
{
    private readonly AppDbContext _context;
    // Opcional: private readonly IConfiguration _configuration;

    public EmailService(AppDbContext context /*, IConfiguration configuration */)
    {
        _context = context;
        // Opcional: _configuration = configuration;
    }

    /// <summary>
    /// Envía una confirmación de cita y guarda el historial de correo.
    /// Maneja el caso en que Patient o Doctor sean nulos (si la búsqueda falló).
    /// </summary>
    /// <param name="patient">El paciente (puede ser null).</param>
    /// <param name="doctor">El doctor (puede ser null).</param>
    public async Task SendConfirmationEmail(Appointment appointment, Patient? patient, Doctor? doctor)
    {
        // 1. COMPROBACIÓN CRÍTICA DE NULOS
        if (patient == null)
        {
            Console.WriteLine($"[EMAIL ERROR] No se puede enviar el correo de confirmación. Paciente con ID {appointment.PatientId} no encontrado.");
            return; // Detiene el proceso si no hay paciente
        }
        if (doctor == null)
        {
            Console.WriteLine($"[EMAIL ERROR] No se puede enviar el correo de confirmación. Doctor con ID {appointment.DoctorId} no encontrado.");
            return; // Detiene el proceso si no hay doctor
        }
        
        // 2. Preparar el contenido (Solo se ejecuta si patient y doctor NO son null)
        string subject = "Confirmación de Cita Médica";
        string content = $@"
Estimado(a) {patient.Name},

Su cita médica con el Dr. {doctor.Name} ({doctor.Specialty}) ha sido confirmada.
Fecha y Hora: {appointment.DateTime.ToString("dd/MM/yyyy HH:mm")}
Estado: {appointment.Status}

Por favor, llegue 15 minutos antes de la hora programada.
";

        // 3. Simulación de Envío (Aquí iría la lógica SMTP real)
        Console.WriteLine($"\n--- EMAIL SIMULADO ENVIADO ---");
        Console.WriteLine($"Destinatario: {patient.Email}");
        Console.WriteLine($"Asunto: {subject}");
        Console.WriteLine("--------------------------------------");

        // 4. Registrar el historial en la base de datos
        try
        {
            var history = new EmailHistory
            {
                Recipient = patient.Email,
                Subject = subject,
                Content = content,
                SentAt = DateTime.Now,
                IsSent = true,
                AppointmentId = appointment.Id 
            };

            _context.EmailHistory.Add(history);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DB ERROR] No se pudo guardar el historial de correo para la cita {appointment.Id}. Error: {ex.Message}");
        }
    }
}