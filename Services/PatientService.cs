using Microsoft.EntityFrameworkCore;
using Hospital_San_Vicente.Data;
using Hospital_San_Vicente.Models;
using System.Threading.Tasks;

namespace Hospital_San_Vicente.Services;

public class PatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> RegisterPatient(string document, string name, string phone, string email)
    {
        // 1. Verificar si el documento ya existe
        bool exists = await _context.Patients.AnyAsync(p => p.Document == document);
        if (exists)
        {
            throw new InvalidOperationException($"Ya existe un paciente con el documento: {document}.");
        }

        // 2. Crear y guardar el nuevo paciente
        var newPatient = new Patient
        {
            Document = document,
            Name = name,
            Phone = phone,
            Email = email
            // Asegúrate de que tu modelo Patient tenga un constructor sin parámetros
        };

        _context.Patients.Add(newPatient);
        await _context.SaveChangesAsync();
        
        return newPatient;
    }

    public async Task<Patient?> GetPatientById(int id) =>
        await _context.Patients.FindAsync(id);
    
    // Puedes añadir más métodos: UpdatePatient, DeletePatient, etc.
}