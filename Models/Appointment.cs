using System; 
using System.Collections.Generic;

namespace Hospital_San_Vicente.Models;

public class Appointment 
{ 
    public int Id { get; set; } 
    public int PatientId { get; set; } 
    public int DoctorId { get; set; } 
    public DateTime DateTime { get; set; } 
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; 
    
    // Navigation properties
    public Patient Patient { get; set; } 
    public Doctor Doctor { get; set; } 
    public ICollection<EmailHistory> EmailLogs { get; set; } = new List<EmailHistory>(); 
}