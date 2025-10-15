using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Hospital_San_Vicente.Data;
using Hospital_San_Vicente.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.IO;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // 1. Crear y Configurar el Host
        var host = CreateHostBuilder(args).Build();

        // 2. Realizar tareas de inicio de la aplicación
        using (var scope = host.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                // ** Migración de Base de Datos y Comprobación de Conexión **
                var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("¡Configuración exitosa! La base de datos ha sido migrada.");
                
                // Los servicios necesarios para el menú se resuelven aquí
                var appointmentService = serviceProvider.GetRequiredService<AppointmentService>();
                var doctorService = serviceProvider.GetRequiredService<DoctorService>();
                var patientService = serviceProvider.GetRequiredService<PatientService>();
                
                Console.WriteLine("\nIniciando aplicación. Presiona Ctrl+C para salir en cualquier momento.");
                
                // === INICIO DEL BUCLE DEL MENÚ INTERACTIVO ===
                await RunMenu(appointmentService, doctorService, patientService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=========================================");
                Console.WriteLine($"Ocurrió un error crítico durante la ejecución:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Detalle: {ex.InnerException?.Message ?? "No hay detalles internos."}");
                Console.WriteLine($"=========================================\n");
            }
        }
    }
    private static async Task RunMenu(AppointmentService appointmentService, DoctorService doctorService, PatientService patientService)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- HOSPITAL SAN VICENTE - MENÚ ---");
            Console.WriteLine("1. Registrar nuevo paciente");
            Console.WriteLine("2. Programar nueva cita");
            Console.WriteLine("3. Listar citas por paciente (ID 1)");
            Console.WriteLine("4. Registrar Doctor"); 
            Console.WriteLine("5. Salir");
            
            Console.Write("Seleccione una opción: ");
            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice.Trim())
                {
                    case "1":
                        Console.WriteLine("\n--- REGISTRAR NUEVO PACIENTE ---");

                        Console.Write("Documento: ");
                        string document = Console.ReadLine() ?? string.Empty;

                        Console.Write("Nombre completo: ");
                        string name = Console.ReadLine() ?? string.Empty;

                        Console.Write("Teléfono: ");
                        string phone = Console.ReadLine() ?? string.Empty;

                        Console.Write("Email: ");
                        string email = Console.ReadLine() ?? string.Empty;
                        
                        var newPatient = await patientService.RegisterPatient(document, name, phone, email);

                        Console.WriteLine($"\n Paciente registrado con éxito. ID: {newPatient.Id}, Nombre: {newPatient.Name}");
                        break;
                    case "2":
                        Console.WriteLine("\n--- PROGRAMAR NUEVA CITA ---");
                        
                        Console.Write("Ingrese el ID del Doctor: ");
                        if (!int.TryParse(Console.ReadLine(), out int doctorId))
                        {
                            Console.WriteLine("ID de doctor no válido.");
                            break;
                        }
                        
                        Console.Write("Ingrese el ID del Paciente: ");
                        
                        if (!int.TryParse(Console.ReadLine(), out int patientId))
                        {
                            Console.WriteLine("ID de paciente no válido.");
                            break;
                        }

                        Console.Write("Ingrese Fecha y Hora de la cita (Ej: 2025-10-30 14:00): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime appointmentDateTime))
                        {
                            Console.WriteLine("Formato de fecha u hora no válido.");
                            break;
                        }
    
                        var newAppointment = await appointmentService.ScheduleAppointment(
                            patientId, 
                            doctorId, 
                            appointmentDateTime
                        );

                        Console.WriteLine($"\n Cita programada con éxito. ID: {newAppointment.Id}. Se envió la confirmación por email simulado.");
                        break;
                    case "3":
                        Console.WriteLine("\n--- LISTAR CITAS POR PACIENTE ---");

                        Console.Write("Ingrese el ID del Paciente para listar sus citas: ");
                        if (!int.TryParse(Console.ReadLine(), out int patientToListId))
                            
                        {
                            Console.WriteLine("ID de paciente no válido.");
                            break;
                        }
                        
                        var appointments = await appointmentService.ListAppointmentsByPatient(patientToListId);

                        if (appointments.Count == 0)
                        {
                            Console.WriteLine($"\nℹ El paciente {patientToListId} no tiene citas programadas.");
                        }
                        else
                        {
                            Console.WriteLine($"\n--- Citas para Paciente ID {patientToListId} ({appointments.Count} encontradas) ---");
                            foreach (var appt in appointments)
                            {
                              
                                Console.WriteLine($"- ID: {appt.Id} | Doctor: {appt.Doctor?.Name ?? "N/A"} | Fecha: {appt.DateTime.ToString("dd/MM/yyyy HH:mm")} | Estado: {appt.Status}");
                            }
                        }
                        break;
                    case "4":
                        Console.WriteLine("\n--- REGISTRAR NUEVO DOCTOR ---");

                        Console.Write("Documento: ");
                        string docDoctor = Console.ReadLine() ?? string.Empty;

                        Console.Write("Nombre completo: ");
                        string nameDoctor = Console.ReadLine() ?? string.Empty;

                        Console.Write("Especialidad: ");
                        string specialtyDoctor = Console.ReadLine() ?? string.Empty;

                        var newDoctor = await doctorService.RegisterDoctor(docDoctor, nameDoctor, specialtyDoctor);

                        Console.WriteLine($"\n Doctor registrado con éxito. ID: {newDoctor.Id}, Nombre: {newDoctor.Name}");
                        break;

                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar la opción: {ex.Message}");
            }
        }
        Console.WriteLine("\nAplicación finalizada. ¡Adiós!");
    }

    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                });

                // === REGISTRO DE TODOS LOS SERVICIOS ===
                services.AddScoped<AppointmentService>();
                services.AddScoped<DoctorService>();
                services.AddScoped<EmailService>();
                services.AddScoped<PatientService>(); 
            });
}