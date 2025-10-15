using Microsoft.EntityFrameworkCore;
using Hospital_San_Vicente.Data;
using Hospital_San_Vicente.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital_San_Vicente.Services;

public class DoctorService
{
    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor> RegisterDoctor(string document, string name, string specialty)
    {
        // 1. Verificar si el documento ya existe
        bool exists = await _context.Doctors.AnyAsync(d => d.Document == document);
        if (exists)
        {
            throw new InvalidOperationException($"Ya existe un doctor con el documento: {document}.");
        }

        // 2. Crear y guardar el nuevo doctor
        var newDoctor = new Doctor
        {
            Document = document,
            Name = name,
            Specialty = specialty
        };

        _context.Doctors.Add(newDoctor);
        await _context.SaveChangesAsync();
        
        return newDoctor;
    }
    
    public async Task<Doctor?> GetDoctorById(int id) =>
        await _context.Doctors.FindAsync(id);

    public async Task<List<Doctor>> ListAllDoctors() =>
        await _context.Doctors.ToListAsync();
}