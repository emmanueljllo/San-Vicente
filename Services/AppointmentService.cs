using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Hospital_San_Vicente.Models;
using Hospital_San_Vicente.Data;  

namespace Hospital_San_Vicente.Services; 

public class AppointmentService
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public AppointmentService(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Appointment> ScheduleAppointment(int patientId, int doctorId, DateTime appointmentDateTime)
    {
        try
        {
            // 1. Validaciones de Conflicto de Horario
            var doctorConflict = await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && a.DateTime == appointmentDateTime && a.Status == AppointmentStatus.Pending);
            if (doctorConflict) {
                throw new InvalidOperationException("El médico ya tiene una cita pendiente programada exactamente a esta hora.");
            }
            
            var patientConflict = await _context.Appointments
                .AnyAsync(a => a.PatientId == patientId && a.DateTime == appointmentDateTime && a.Status == AppointmentStatus.Pending);
            if (patientConflict) {
                throw new InvalidOperationException("El paciente ya tiene una cita pendiente programada exactamente a esta hora.");
            }
            
            // 2. Crear y guardar la nueva cita
            var newAppointment = new Appointment { PatientId = patientId, DoctorId = doctorId, DateTime = appointmentDateTime };
            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            // 3. Buscar Paciente y Doctor para el correo (CRÍTICO)
            var patient = await _context.Patients.FindAsync(patientId);
            var doctor = await _context.Doctors.FindAsync(doctorId);

            // ** VERIFICACIÓN DE NULOS ANTES DE USAR **
            if (patient == null)
            {
                // La cita se creó, pero el paciente no existe (un error de datos).
                // Podríamos revertir la creación de la cita, pero por ahora solo lanzamos la excepción.
                throw new KeyNotFoundException($"La cita se creó, pero el Paciente con ID {patientId} no fue encontrado para el correo.");
            }
            if (doctor == null)
            {
                throw new KeyNotFoundException($"La cita se creó, pero el Doctor con ID {doctorId} no fue encontrado para el correo.");
            }

            // 4. Llamar al servicio de email (ahora es seguro)
            await _emailService.SendConfirmationEmail(newAppointment, patient, doctor);
            
            return newAppointment;
        } 
        catch (KeyNotFoundException)
        {
             // Propagar la excepción de "no encontrado"
            throw;
        }
        catch (InvalidOperationException)
        {
            // Propagar la excepción de conflicto de horario
            throw;
        }
        catch (Exception ex) 
        {
            // Capturar y relanzar cualquier otro error de base de datos
            throw new Exception($"La programación de la cita falló: {ex.Message}");
        }
    }
    
    public async Task CancelAppointment(int appointmentId) 
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new KeyNotFoundException("Cita no encontrada.");
        if (appointment.Status != AppointmentStatus.Pending) throw new InvalidOperationException("Solo las citas Pendientes pueden ser canceladas.");
        appointment.Status = AppointmentStatus.Canceled;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAttended(int appointmentId) 
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new KeyNotFoundException("Cita no encontrada.");
        if (appointment.Status != AppointmentStatus.Pending) throw new InvalidOperationException("Solo las citas Pendientes pueden marcarse como Atendidas.");
        appointment.Status = AppointmentStatus.Attended;
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Appointment>> ListAppointmentsByPatient(int patientId) => 
        await _context.Appointments
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Doctor)
            .ToListAsync();

    public async Task<List<Appointment>> ListAppointmentsByDoctor(int doctorId) => 
        await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .Include(a => a.Patient)
            .ToListAsync();
}